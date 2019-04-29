using System;

public class GoldResource : Resource
{
    private static readonly float maxCapacity = 100;

    protected override float MaxCapacity => maxCapacity;

    public override void Gather(Unit worker)
    {
        float amount = Math.Min(worker.Gathering, capacity);
        worker.owner.Gather(amount, this);
        PlayerState.Instance.Gold += amount;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<GoldResource>(this);
        return miningJob;
    }
}
