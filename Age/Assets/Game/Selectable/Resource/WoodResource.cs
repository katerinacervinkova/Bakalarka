using System;

public class WoodResource : Resource
{
    private static readonly int maxCapacity = 100;

    protected override float MaxCapacity => maxCapacity;

    public override void Gather(float gathering, Player player)
    {
        float amount = Math.Min(gathering, capacity);
        player.Gather(amount, this);
        PlayerState.Instance.Wood += amount;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<WoodResource>(this);
        return miningJob;
    }
}