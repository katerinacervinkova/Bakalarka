using System;

public class FoodResource : Resource
{
    public override string Name => "Berries";

    private static readonly float maxCapacity = 100;

    protected override float MaxCapacity => maxCapacity;

    public override void Gather(float gathering, Player player)
    {
        float amount = Math.Min(gathering, capacity);
        player.Gather(amount, this);
        PlayerState.Instance.Food += amount;
    }

    public override Job GetEnemyJob(Commandable worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<FoodResource>(this);
        return miningJob;
    }
}