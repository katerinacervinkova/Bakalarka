using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Selectable {

    public override string GetObjectDescription() => $"{base.GetObjectDescription()}\n{unitsInside.Count} unit(s) inside";

    /// <summary>
    /// Destination for the unit which exits the building
    /// </summary>
    public Vector3 DefaultDestination { get; private set; }

    // variables linked to the units inside the building
    public virtual int UnitCapacity => 5;
    protected List<Unit> unitsInside = new List<Unit>();
    public int UnitCount => unitsInside.Count;

    /// <summary>
    /// name under which the unit will be known inside building
    /// </summary>
    public virtual string UnitName(Unit unit) => unit.Name;
    /// <summary>
    /// text by which the unit will be described inside building
    /// </summary>
    public abstract string UnitText(Unit unit);

    // list of queued transactions
    public List<Transaction> transactions = new List<Transaction>();
    private readonly int maxTransactions = 16;
    private Transaction activeTransaction;

    private readonly float minTime = 1;
    private float timeElapsed = 0;

    /// <summary>
    /// number by which the maximum population of the player is increased by owning this building
    /// </summary>
    protected virtual int MaxPopulationIncrease { get; } = 0;

    /// <summary>
    /// Updates unit inside building.
    /// </summary>
    protected abstract void UpdateUnit(Unit unit);

    /// <summary>
    /// Changes the model color so that everyone knows who this building belongs to.
    /// </summary>
    protected abstract void ChangeColor();

    /// <summary>
    /// Overrides default implementation, because the model is not created with the component
    /// and initialization has to be made somewhere else.
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// Updates the unit window.
    /// </summary>
    public void OnUnitsChange()
    {
        UIManager.Instance.HideBuildingWindow();
        ShowUnitsWindow();
    }

    /// <summary>
    /// Updates all units inside building and corresponding UI.
    /// </summary>
    protected override void Update()
    {
        base.Update();
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            foreach (Unit unit in unitsInside)
                UpdateUnit(unit);

            if (UIManager.Instance.BuildingWindowShown == this)
                UIManager.Instance.UpdateBuildingWindowDescriptions();

            timeElapsed -= minTime;
        }
    }

    public override void Init()
    {
        base.Init();
        GameState.Instance.Buildings.Add(this);
        GameState.Instance.UpdateGraph(GetComponent<Collider>().bounds);

        healthBar = UIManager.Instance.CreateHealthBar(this, healthBarOffset);

        minimapColor = owner.color;
        minimapIcon.color = minimapColor; 

        DefaultDestination = FrontPosition;

        visibleObject.SetActive(false);
        if (hasAuthority)
        {
            SetVisibility(true);
            PlayerState.Get(playerId).buildings.Add(this);
            PlayerState.Get(playerId).MaxPopulation += MaxPopulationIncrease;
            InitPurchases();
        }
        ChangeColor();

        visibleObject.transform.Find("Building").gameObject.SetActive(true);
        Destroy(visibleObject.transform.Find("Fence").gameObject);
        Destroy(visibleObject.transform.Find("Image").gameObject);

        initialized = true;
    }

    protected override void ShowAllButtons()
    {
        base.ShowAllButtons();
        UIManager.Instance.ShowTransactions(transactions);
        UIManager.Instance.ShowTarget(DefaultDestination);
        UIManager.Instance.ShowBuildingWindowButton();
        UIManager.Instance.ShowDestroyButton();
    }

    protected override void HideAllButtons()
    {
        base.HideAllButtons();
        UIManager.Instance.HideTransactions();
        UIManager.Instance.HideTarget();
        UIManager.Instance.HideBuildingWindowButton();
        UIManager.Instance.HideDestroyButton();
    }

    /// <summary>
    /// Makes unit enter the building if there is capacity for it.
    /// </summary>
    /// <param name="unit">unit to enter the building</param>
    /// <returns>true if succeeded</returns>
    public bool Enter(Unit unit)
    {
        if (unitsInside.Count + 1 >= UnitCapacity)
            return false;
        unitsInside.Add(unit);
        return true;
    }

    /// <summary>
    /// Makes unit exit the building.
    /// </summary>
    public void Exit(Unit unit)
    {
        if (unitsInside.Remove(unit))
            owner.ExitBuilding(unit, this);
        PlayerState.Get(playerId).OnStateChange(this);
    }

    /// <summary>
    /// Makes all units exit the building.
    /// </summary>
    public void Exit()
    {
        unitsInside.ForEach(u => owner.ExitBuilding(u, this));
        unitsInside.Clear();
        PlayerState.Get(playerId).OnStateChange(this);
    }

    public virtual void ShowUnitsWindow()
    {
        UIManager.Instance.ShowBuildingWindow(this, unitsInside);
    }

    /// <summary>
    /// Adds given transaction to the list and starts it if there is no other transaction.
    /// </summary>
    /// <param name="transaction"></param>
    public void AddTransaction(Transaction transaction)
    {
        transactions.Add(transaction);
        if (activeTransaction == null)
        {
            activeTransaction = transaction;
            StartTransaction();
        }
    }

    public void StartTransaction()
    {
        StartCoroutine("LoadTransaction");
    }

    /// <summary>
    /// Called when the player wants to reset the transaction.
    /// Removes it from the list and cancels it if it is already active. 
    /// </summary>
    /// <param name="index">index of the transaction</param>
    public void RemoveTransaction(int index)
    {
        transactions[index].Reset();
        if (index == 0)
            FinishTransaction();
        else
            transactions.RemoveAt(index);
        PlayerState.Get(playerId).OnTransactionLoading(this);
    }

    /// <summary>
    /// Loads transaction. If the loading is complete, finishes it.
    /// </summary>
    private IEnumerator LoadTransaction()
    {
        while (true)
        {
            if (activeTransaction.Load(Time.deltaTime))
                FinishTransaction();
            PlayerState.Get(playerId).OnTransactionLoading(this);
            yield return null;
        }
    }

    /// <summary>
    /// Ends the current transaction and if there is some other one, starts it.
    /// </summary>
    private void FinishTransaction()
    {
        StopAllCoroutines();
        activeTransaction = null;
        transactions.RemoveAt(0);
        if (transactions.Count > 0)
        {
            activeTransaction = transactions[0];
            StartTransaction();
        }
    }

    /// <summary>
    /// Returns true if the maximum number of transaction is yet to be reached.
    /// </summary>
    public bool CanStartTransaction()
    {
        return transactions.Count < maxTransactions;
    }

    /// <summary>
    /// Sets the default destination to the given point.
    /// </summary>
    public override void SetGoal(Vector3 hitPoint)
    {
        if (!hasAuthority || !owner.IsHuman)
            return;
        DefaultDestination = hitPoint;
        UIManager.Instance.ShowTarget(DefaultDestination);
    }

    public override Job GetOwnJob(Unit worker)
    {
        return new JobEnter(this);
    }

    public void SetHealthBar(HealthBar healthBar)
    {
        this.healthBar = healthBar;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (hasAuthority)
        {
            if (PlayerState.Get(playerId) != null)
            {
                PlayerState.Get(playerId).buildings.Remove(this);
                PlayerState.Get(playerId).MaxPopulation -= MaxPopulationIncrease;
            }

            // make all units exit the building
            unitsInside.ForEach(u => owner.ExitBuilding(u, this));
        }
        GameState.Instance?.RemoveFromSquare(SquareID, this);
        GameState.Instance?.Buildings.Remove(this);
    }
}