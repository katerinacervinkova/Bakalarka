using System;

public class WoodResource : Resource
{
    private static readonly int maxCapacity = 100;

    protected override int MaxCapacity => maxCapacity;

    public override void Mine(Unit worker)
    {
        int amount = Math.Min(worker.Strength, capacity);
        worker.owner.Mine(amount, this);
        PlayerState.Instance.Wood += amount;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobMine<WoodResource>(this);
        return miningJob;
    }
}