using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Selectable {
    public Vector3 DefaultDestination { get; private set; }

    private readonly int maxTransactions = 16;
    private Transaction activeTransaction;
    public List<Transaction> transactions = new List<Transaction>();

    protected List<Unit> unitsInside = new List<Unit>();
    protected int unitCapacity = 10;

    private readonly float minTime = 1;
    private float timeElapsed = 0;

    protected abstract void UpdateUnit(Unit unit);
    public abstract Func<Unit, string> UnitTextFunc { get; }

    public override void OnStartClient()
    {
        GameState.Instance.Buildings.Add(this);
        GameState.Instance.UpdateGraph(GetComponent<Collider>().bounds);
    }

    public void OnUnitsChange()
    {
        UIManager.Instance.HideBuildingWindow();
        ShowUnitsWindow();
    }

    protected virtual void Update()
    {
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
        minimapColor = owner.color;
        minimapIcon.color = minimapColor; 
        DefaultDestination = FrontPosition;
        GameState.Instance.Buildings.Add(this);
        if (hasAuthority)
        {
            PlayerState.Instance.buildings.Add(this);
            InitPurchases();
        }
        Debug.DrawRay(FrontPosition, Vector3.up * 10, Color.green, 5000);
        initialized = true;
    }


    public override void SetSelection(bool selected, Player player)
    {
        base.SetSelection(selected, player);
        if (UIManager.Instance != null)
        {
            if (selected)
            {
                UIManager.Instance.ShowTransactions(transactions);
                UIManager.Instance.ShowTarget(DefaultDestination);
                UIManager.Instance.ShowUnitsButton(ShowUnitsWindow);
            }
            else
            {
                UIManager.Instance.HideTransactions();
                UIManager.Instance.HideTarget();
                UIManager.Instance.HideUnitsButton();
            }
        }
    }

    public bool Enter(Unit unit)
    {
        if (unitsInside.Count + 1 >= unitCapacity)
            return false;
        unitsInside.Add(unit);
        return true;
    }

    public void Exit(Unit unit)
    {
        if (unitsInside.Remove(unit))
            owner.ExitBuilding(unit, this);
        PlayerState.Instance.OnStateChange(this);
    }

    public void Exit()
    {
        unitsInside.ForEach(u => owner.ExitBuilding(u, this));
        unitsInside.Clear();
        PlayerState.Instance.OnStateChange(this);
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
        PlayerState.Instance.OnTransactionLoading(this);
    }

    private IEnumerator LoadTransaction()
    {
        while (true)
        {
            if (activeTransaction.Load(Time.deltaTime))
                FinishTransaction();
            PlayerState.Instance.OnTransactionLoading(this);
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerState.Instance?.buildings.Remove(this);
        GameState.Instance?.Buildings.Remove(this);
        GameState.Instance?.UpdateGraph(GetComponent<Collider>().bounds);
    }
}