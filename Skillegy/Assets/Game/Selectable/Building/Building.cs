using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Selectable {

    public Vector3 DefaultDestination { get; private set; }

    public virtual int UnitCapacity => 5;
    public int UnitCount => unitsInside.Count;

    public List<Transaction> transactions = new List<Transaction>();
    public virtual string UnitName(Unit unit) => unit.Name;
    public abstract string UnitText(Unit unit);


    private readonly int maxTransactions = 16;
    private Transaction activeTransaction;

    protected List<Unit> unitsInside = new List<Unit>();

    private readonly float minTime = 1;
    private float timeElapsed = 0;

    protected virtual int MaxPopulationIncrease { get; } = 0;

    protected abstract void UpdateUnit(Unit unit);

    protected abstract void ChangeColor();


    public override void OnStartClient() { }

    public void OnUnitsChange()
    {
        UIManager.Instance.HideBuildingWindow();
        ShowUnitsWindow();
    }

    protected override void Update()
    {
        base.Update();
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            foreach (Unit unit in unitsInside)
                UpdateUnit(unit);
            timeElapsed -= minTime;
            if (UIManager.Instance.BuildingWindowShown == this)
                UIManager.Instance.UpdateBuildingWindowDescriptions();
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

    public bool Enter(Unit unit)
    {
        if (unitsInside.Count + 1 >= UnitCapacity)
            return false;
        unitsInside.Add(unit);
        return true;
    }

    public void Exit(Unit unit)
    {
        if (unitsInside.Remove(unit))
            owner.ExitBuilding(unit, this);
        PlayerState.Get(playerId).OnStateChange(this);
    }

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

    internal void RemoveTransaction(int index)
    {
        transactions[index].Reset();
        if (index == 0)
            FinishTransaction();
        else
            transactions.RemoveAt(index);
        PlayerState.Get(playerId).OnTransactionLoading(this);
    }

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

    public bool CanStartTransaction()
    {
        return transactions.Count < maxTransactions;
    }

    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        if (!hasAuthority || !owner.IsHuman)
            return;
        DefaultDestination = hitPoint;
        UIManager.Instance.ShowTarget(DefaultDestination);
    }

    public override string GetObjectDescription()
    {
        return $"{base.GetObjectDescription()}\n{unitsInside.Count} unit(s) inside";
    }

    public override Job GetOwnJob(Commandable worker)
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
            unitsInside.ForEach(u => owner.ExitBuilding(u, this));
        }
        GameState.Instance?.RemoveFromSquare(SquareID, this);
        GameState.Instance?.Buildings.Remove(this);
    }
}