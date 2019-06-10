using System;

public class WoodResource : Resource
{
    public override string Name => "Tree";

    private static readonly int maxCapacity = 100;

    protected override float MaxCapacity => maxCapacity;

    public override bool Gather(float gathering, Player player)
    {
        bool completed = capacity - gathering <= 0;
        float amount = Math.Min(gathering, capacity);
        player.Gather(amount, this);
        PlayerState.Get(playerId).Wood += amount;
        return completed;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<WoodResource>(this);
        return miningJob;
    }
}