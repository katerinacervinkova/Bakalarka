using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : Selectable {

    private Job miningJob = null;

    private float capacity;
    protected abstract int MaxCapacity();
    private float Capacity
    {
        get { return capacity; }
        set { capacity = value; EventManager.TriggerEvent(this, miningEvent); }
    }


    protected override void Start()
    {
        base.Start();
        capacity = MaxCapacity();
    }

    public static readonly string miningEvent = "capacityChanged";

    public override void DrawHealthBar()
    {
        DrawProgressBar(1);
    }

    protected override Job CreateEnemyJob(Commandable worker)
    {
        return null;
    }

    protected override Job CreateOwnJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobMine(this);
        return miningJob;
    }

    protected override void DrawSelectedObjectText()
    {
        if (Selected)
            selectedObjectText.text = string.Format("Capacity: {0}/{1}", Capacity, MaxCapacity());
    }

    public void Mine(Unit worker)
    {
        Capacity -= worker.Strength;
    }

    protected override void RemoveEvents()
    {
        EventManager.StopAllListening(this, miningEvent);
    }

    protected override void SetEvents()
    {
        EventManager.StartListening(this, miningEvent, DrawHealthBar);
        EventManager.StartListening(this, miningEvent, DrawSelectedObjectText);
        EventManager.StartListening(this, miningEvent, ControlCapacity);
    }

    protected void ControlCapacity()
    {
        miningJob.Completed = Capacity <= 0;
        if (miningJob.Completed)
            Destroy(gameObject);
    }
}
