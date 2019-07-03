using UnityEngine;

public class Unit : Commandable
{
    public override string Name => "Unit";

    protected AIUnetPath aiUnetPath;

    public MovementController movementController;
    public Regiment Reg { get; set; }
    private Attributes atts;

    public float GetAttribute(AttEnum attEnum) => atts.Get(attEnum);
    public void SetAttribute(AttEnum attEnum, float value) => atts.Set(attEnum, value);

    public float Gathering { get { return atts.Get(AttEnum.Gathering); } set { atts.Set(AttEnum.Gathering, value); } }
    public float Intelligence { get { return atts.Get(AttEnum.Intelligence); } set { atts.Set(AttEnum.Intelligence, value); } }
    public float Swordsmanship { get { return atts.Get(AttEnum.Swordsmanship); } set { atts.Set(AttEnum.Swordsmanship, value); } }
    public float Healing { get { return atts.Get(AttEnum.Healing); } set { atts.Set(AttEnum.Healing, value); } }
    public float Building { get { return atts.Get(AttEnum.Building); } set { atts.Set(AttEnum.Building, value); } }
    public float Accuracy { get { return atts.Get(AttEnum.Accuracy); } set { atts.Set(AttEnum.Accuracy, value); } }

    public float Range => 5;
    private Job Job { get; set; }

    public bool HasJob => !(Job is JobLookForTarget);
    protected void Awake()
    {
        atts = GetComponent<Attributes>();
        aiUnetPath = GetComponent<AIUnetPath>();
        movementController = GetComponent<MovementController>();
    }

    public override void Init()
    {
        base.Init();
        minimapColor = owner.color;
        minimapIcon.color = minimapColor;
        GameState.Instance.Units.Add(this);
        visibleObject.transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = owner.color;
        healthBar = UIManager.Instance.CreateHealthBar(this, healthBarOffset);
        SetVisibility(false);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        if (PlayerState.Get(playerId).units.Count == 0)
            owner.StartTheGame();
        PlayerState.Get(playerId).units.Add(this);
        Job = new JobLookForTarget();
        SetVisibility(true);
    }

    protected override void Update()
    {
        base.Update();
        JobUpdate();
        GameState.Instance.PositionChange(this);
    }

    private void OnDisable()
    {
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
    }

    protected override void Cover()
    {
        base.Cover();
        visibleObject.SetActive(false);
    }

    public override void DealAttack(Selectable selectable) => SetJob(new JobAttack(selectable));

    public override void SetSelection(bool selected)
    {
        base.SetSelection(selected);
        if (selected && IsMoving)
            ShowTarget();
        else
            HideTarget();
    }

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

    protected virtual void JobUpdate()
    {
        if (!hasAuthority)
            return;
        if (Job != null && Job.Completed)
            SetJob(Job.Following);
        Job?.Do(this);
    }

    
    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        if (!hasAuthority || !owner.IsHuman)
            return;
        SetJob(new JobGo(hitPoint));
    }

    public override string GetObjectDescription() => $"{base.GetObjectDescription()}\n{atts.GetDescription()}";

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
        HideTarget();
        destination = Vector3.positiveInfinity;
        if (job == null)
            job = new JobLookForTarget();
        Job = job;
    }

    public void SetNextJob() => SetJob(Job.Following);

    public void ResetJob() => SetJob(null);

    public void Go(Vector3 destination)
    {
        this.destination = destination;
        ShowTarget();
        aiUnetPath.destination = destination;
        aiUnetPath.endReachedDistance = 0.6f;
    }

    protected override void InitPurchases()
    {
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.House));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Barracks));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Mill));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Sawmill));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.MainBuilding));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Infirmary));
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Library));
    }

    public void OnTargetReached()
    {
        destination = Vector3.positiveInfinity;
        Reg?.MovementCompleted(this);
        if (Job is JobGo)
            Job.Completed = true;
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