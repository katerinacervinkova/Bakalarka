using UnityEngine;

public class Unit : Selectable
{
    public override string Name => "Unit";
    public override string GetObjectDescription() => $"{base.GetObjectDescription()}\n{skills.GetDescription()}";

    public MovementController movementController;

    // regiment the unit currently belongs to
    public Regiment Reg { get; set; }

    //variables related to skills
    private Skills skills;

    public float GetSkillLevel(SkillEnum skill) => skills.Get(skill);
    public void SetSkillLevel(SkillEnum skill, float value) => skills.Set(skill, value);

    public float Gathering { get { return skills.Get(SkillEnum.Gathering); } set { skills.Set(SkillEnum.Gathering, value); } }
    public float Intelligence { get { return skills.Get(SkillEnum.Intelligence); } set { skills.Set(SkillEnum.Intelligence, value); } }
    public float Swordsmanship { get { return skills.Get(SkillEnum.Swordsmanship); } set { skills.Set(SkillEnum.Swordsmanship, value); } }
    public float Healing { get { return skills.Get(SkillEnum.Healing); } set { skills.Set(SkillEnum.Healing, value); } }
    public float Building { get { return skills.Get(SkillEnum.Building); } set { skills.Set(SkillEnum.Building, value); } }

    // variables related to jobs
    private Job Job { get; set; }
    public bool HasJob => !(Job is JobLookForTarget);
    public float Range => 5;


    protected void Awake()
    {
        skills = GetComponent<Skills>();
        movementController = GetComponent<MovementController>();
    }

    public override void Init()
    {
        base.Init();

        // set the color
        minimapColor = owner.color;
        minimapIcon.color = minimapColor;
        visibleObject.transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = owner.color;

        GameState.Instance.Units.Add(this);
        healthBar = UIManager.Instance.CreateHealthBar(this, healthBarOffset);

        // an enemy should see the unit only if it's close to him
        SetVisibility(false);
    }

    protected override void InitPurchases()
    {
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.House));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Barracks));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Mill));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Sawmill));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Bank));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.MainBuilding));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Infirmary));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Library));
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // if this is the player's first unit, start the game
        if (PlayerState.Get(playerId).units.Count == 0)
            owner.StartTheGame();

        PlayerState.Get(playerId).units.Add(this);

        // look for target is a default job
        Job = new JobLookForTarget();

        // unit's owner should see his own unit
        SetVisibility(true);
    }

    protected override void Update()
    {
        base.Update();
        JobUpdate();
        GameState.Instance.PositionChange(this);
    }

    public override void SetSelection(bool selected)
    {
        base.SetSelection(selected);
        if (selected && movementController.IsMoving)
            movementController.ShowTarget();
        else
            movementController.HideTarget();
    }

    protected override void Cover()
    {
        base.Cover();
        visibleObject.SetActive(false);
    }
   

    /// <summary>
    /// Called when the unit has just reached its destination.
    /// </summary>
    public void OnTargetReached()
    {
        
        Reg?.MovementCompleted(this);
        if (Job is JobGo)
            Job.Completed = true;
    }

    /// <summary>
    /// Perform the counter attack.
    /// </summary>
    public override void DealAttack(Selectable selectable) => SetJob(new JobAttack(selectable));

    /// <summary>
    /// Makes unit reappear.
    /// </summary>
    /// <param name="position"></param>
    public void ExitBuilding(Vector3 position)
    {
        transform.position = position;
        SetVisibility(true);
        gameObject.SetActive(true);
    }

    protected override void ShowAllButtons()
    {
        base.ShowAllButtons();
        UIManager.Instance.ShowDestroyButton();
    }

    protected override void HideAllButtons()
    {
        base.HideAllButtons();
        UIManager.Instance.HideDestroyButton();
    }

    /// <summary>
    /// Changes the job if it is completed and then does the job.
    /// </summary>
    protected virtual void JobUpdate()
    {
        if (!hasAuthority)
            return;
        if (Job != null && Job.Completed)
            SetJob(Job.Following);
        Job?.Do(this);
    }

    /// <summary>
    /// Sets new Job as the moving to the given destination.
    /// </summary>
    public override void SetGoal(Vector3 hitPoint)
    {
        if (!hasAuthority || !owner.IsHuman)
            return;
        SetJob(new JobGo(hitPoint));
    }

    /// <summary>
    /// Sets the job to get to the goal and perform corresponding action.
    /// </summary>
    public override void SetGoal(Selectable goal)
    {
        if (hasAuthority)
        {
            if (goal.owner == owner)
                SetJob(new JobGo(goal.FrontPosition, goal.GetOwnJob(this)));
            else
                SetJob(new JobFollow(goal, goal.GetEnemyJob(this)));
        }
    }

    public void SetJob(Job job)
    {
        movementController.HideTarget();
        movementController.destination = Vector3.positiveInfinity;
        if (job == null)
            job = new JobLookForTarget();
        Job = job;
    }

    public void SetNextJob() => SetJob(Job.Following);

    public void ResetJob() => SetJob(null);


    public void Go(Vector3 destination)
    {
        movementController.Go(destination);
    }

    public override Job GetOwnJob(Unit worker = null) => null;

    private void OnDisable()
    {
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Reg?.Remove(this);
        if (hasAuthority && PlayerState.Get(playerId) != null)
        {
            PlayerState.Get(playerId).units.Remove(this);
            PlayerState.Get(playerId).Population--;
        }
    }
}