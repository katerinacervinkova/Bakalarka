using System;

public class GoldResource : Resource
{
    public override string Name => "Gold mine";

    private static readonly float maxCapacity = 1000;

    protected override float MaxCapacity => maxCapacity;

    public override bool Gather(float gathering, Player player)
    {
        bool completed = capacity - gathering <= 0;
        float amount = Math.Min(gathering, capacity);
        player.Gather(amount, this);
        PlayerState.Get(playerId).Gold += amount;
        return completed;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<GoldResource>(this);
        return miningJob;
    }
}
