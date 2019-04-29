using System;

public class WoodResource : Resource
{
    private static readonly int maxCapacity = 100;

    protected override float MaxCapacity => maxCapacity;

    public override void Gather(Unit worker)
    {
        float amount = Math.Min(worker.Gathering, capacity);
        worker.owner.Gather(amount, this);
        PlayerState.Instance.Wood += amount;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<WoodResource>(this);
        return miningJob;
    }
}