using System;

public class FoodResource : Resource
{
    public override string Name => "Berries";

    private static readonly float maxCapacity = 100;

    protected override float MaxCapacity => maxCapacity;

    public override bool Gather(float gathering, Player player)
    {
        bool completed = capacity - gathering <= 0;
        float amount = Math.Min(gathering, capacity);
        player.Gather(amount, this);
        PlayerState.Get(player.playerControllerId).Food += amount;
        return completed;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<FoodResource>(this);
        return miningJob;
    }
}