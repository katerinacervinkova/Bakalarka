using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class Resource : Selectable {

    private Job miningJob = null;

    [SyncVar(hook = "OnCapacityChange")]
    private int capacity = 0;
    protected abstract int MaxCapacity();
    public static readonly string miningEvent = "capacityChanged";

    protected void Start()
    {
        capacity = MaxCapacity();
    }
    public override void OnStartClient()
    {
        selector = transform.Find("SelectionProjector").gameObject;
        selector.GetComponent<Projector>().material.color = Color.gray;
        selector.SetActive(false);
        healthBarCanvas = transform.Find("Canvas").gameObject;
        healthBar = transform.Find("Canvas/HealthBarBG/HealthBar").GetComponent<Image>();
        healthBarRotation = healthBarCanvas.transform.rotation;
        InitGameState();
    }
    private void OnCapacityChange(int newCapacity)
    {
        capacity = newCapacity;
        gameState.OnStateChange(this);
        DrawHealthBar();
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar(capacity / MaxCapacity());
    }

    protected override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobMine(this);
        return miningJob;
    }

    protected override Job GetOwnJob(Commandable worker)
    {
        return GetEnemyJob(worker);
    }

    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        selectedObjectText.text = string.Format("Capacity: {0}/{1}", capacity, MaxCapacity());
    }

    [Command]
    public void CmdMine(int strength, NetworkInstanceId playerId)
    {
        int amount = Math.Min(strength, capacity);
        capacity -= amount;
        NetworkServer.objects[playerId].GetComponent<Player>().gold += amount;
        ControlCapacity();
    }

    protected void ControlCapacity()
    {
        var job = GetOwnJob(null);
        job.Completed = capacity <= 0;
        if (job.Completed)
            Destroy(gameObject);
    }

    protected override void InitTransactions() { }
}
