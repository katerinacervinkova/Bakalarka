using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Selectable {

    public Vector3 SpawnPoint { get; private set; }
    public Vector3 DefaultDestination { get; private set; }

    private readonly int maxTransactions = 16;
    private Transaction activeTransaction;
    public List<Transaction> transactions = new List<Transaction>();

    public override void OnStartClient()
    {
        GameState.Instance.Buildings.Add(this);
        GameState.Instance.UpdateGraph(GetComponent<Collider>().bounds);
    }
    public override void Init()
    {
        base.Init();
        minimapColor = owner.color;
        minimapIcon.color = minimapColor; 
        DefaultDestination = SpawnPoint = transform.position;
        DrawHealthBar();
        transform.Find("Floor1").GetComponent<MeshRenderer>().material.color = owner.color;
        transform.Find("Floor2").GetComponent<MeshRenderer>().material.color = owner.color;
        if (hasAuthority)
        {
            PlayerState.Instance.buildings.Add(this);
            InitPurchases();
        }
    }

    public override void SetSelection(bool selected, Player player)
    {
        base.SetSelection(selected, player);
        if (selected)
        {
            UIManager.Instance.ShowTransactions(transactions);
            UIManager.Instance.ShowTarget(DefaultDestination);
        }
        else
        {
            UIManager.Instance.HideTransactions();
            UIManager.Instance.HideTarget();
        }
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
        return "";
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar(Health / (float)MaxHealth);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerState.Instance?.buildings.Remove(this);
        GameState.Instance?.Buildings.Remove(this);
        GameState.Instance?.UpdateGraph(GetComponent<Collider>().bounds);
    }
}