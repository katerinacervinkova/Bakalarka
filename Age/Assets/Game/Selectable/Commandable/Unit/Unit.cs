using UnityEngine;
using UnityEngine.Networking;

public class Unit : Commandable
{

    public override string Name => "Unit";

    protected AIUnetPath aiUnetPath;

    public MovementController movementController;
    public Regiment Reg { get; set; }
    private Attributes atts;

    public float Gathering { get { return atts.Get(AttEnum.Gathering); } set { atts.Set(AttEnum.Gathering, value); } }
    public float Intelligence { get { return atts.Get(AttEnum.Intelligence); } set { atts.Set(AttEnum.Intelligence, value); } }
    public float Swordsmanship { get { return atts.Get(AttEnum.Swordsmanship); } set { atts.Set(AttEnum.Swordsmanship, value); } }
    public float Healing { get { return atts.Get(AttEnum.Healing); } set { atts.Set(AttEnum.Healing, value); } }
    public float Building { get { return atts.Get(AttEnum.Building); } set { atts.Set(AttEnum.Building, value); } }
    public float Accuracy { get { return atts.Get(AttEnum.Accuracy); } set { atts.Set(AttEnum.Accuracy, value); } }

    private Job Job { get; set; }

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
        PlayerState.Instance.units.Add(this);
        SetVisibility(true);
    }

    protected virtual void Update()
    {
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



    public override void SetSelection(bool selected)
    {
        base.SetSelection(selected);
        if (selected && IsMoving)
            ShowTarget();
        else
            HideTarget();
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

    public void SetAttribute(AttEnum attEnum, float value)
    {
        atts.Set(attEnum, value);
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
        if (!hasAuthority)
            return;
        SetJob(new JobGo(hitPoint));
    }

    public override string GetObjectDescription()
    {
        return $"{base.GetObjectDescription()}\n{atts.GetDescription()}";
    }

    public override void SetGoal(Selectable goal)
    {
        Job following = goal.CreateJob(this);
        SetJob(new JobGo(goal.FrontPosition, following));
    }

    public void SetJob(Job job)
    {
        HideTarget();
        destination = Vector3.positiveInfinity;
        Job = job;
    }

    public void SetNextJob()
    {
        SetJob(Job.Following);
    }

    public void ResetJob()
    {
        SetJob(null);
    }

    public void Go(Vector3 destination)
    {
        this.destination = destination;
        ShowTarget();
        aiUnetPath.destination = destination;
        aiUnetPath.endReachedDistance = 0.6f;
    }

    protected override void InitPurchases()
    {
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.House));
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.Barracks));
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.Mill));
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.MainBuilding));
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.Infirmary));
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.Library));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Reg?.Remove(this);
        if (hasAuthority && PlayerState.Instance != null)
        {
            PlayerState.Instance.units.Remove(this);
            PlayerState.Instance.Population--;
        }
    }

    public void OnTargetReached()
    {
        destination = Vector3.positiveInfinity;
        Reg?.MovementCompleted(this);
        if (Job is JobGo)
            Job.Completed = true;
    }
}