using System;

public class FoodResource : Resource
{
    public override string Name => "Berries";

    private static readonly float maxCapacity = 2000;

    protected override float MaxCapacity => maxCapacity;

    /// <summary>
    /// Substracts given amount of food and gives it to the player.
    /// </summary>
    /// <param name="gathering">amount to gather</param>
    /// <param name="player">owner of the gatherer</param>
    /// <returns>true if the berry capacity is now 0</returns>
    public override bool Gather(float gathering, Player player)
    {
        PlayerState.Get(player.playerControllerId).Food += Math.Min(gathering, capacity);
        return base.Gather(gathering, player);
    }

    public override Job GetEnemyJob(Unit worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<FoodResource>(this);
        return miningJob;
    }
}