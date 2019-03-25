using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class Resource : Selectable {

    private Job miningJob = null;

    [SyncVar(hook = "OnCapacityChange")]
    public int capacity = 0;
    protected abstract int MaxCapacity { get; }
    public abstract void Mine(Unit worker);

    protected void Start()
    {
        capacity = MaxCapacity;
    }

    public override void OnStartClient()
    {
        Init();
        minimapColor = minimapIcon.color;
        gameState.AddSelectable(this);
    }

    private void OnCapacityChange(int newCapacity)
    {
        capacity = newCapacity;
        playerState.OnStateChange(this);
        DrawHealthBar();
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar((float)capacity / MaxCapacity);
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
        selectedObjectText.text = string.Format("Capacity: {0}/{1}", capacity, MaxCapacity);
    }

    protected override void InitTransactions() { }
}
