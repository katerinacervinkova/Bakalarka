using System;

public class WoodResource : Resource
{
    public override string Name => "Tree";

    private static readonly int maxCapacity = 1000;

    protected override float MaxCapacity => maxCapacity;

    /// <summary>
    /// Substracts given amount of wood and gives it to the player.
    /// </summary>
    /// <param name="gathering">amount to gather</param>
    /// <param name="player">owner of the gatherer</param>
    /// <returns>true if the tree capacity is now 0</returns>
    public override bool Gather(float gathering, Player player)
    {
        PlayerState.Get(player.playerControllerId).Wood += Math.Min(gathering, capacity);
        return base.Gather(gathering, player);
    }

    public override Job GetEnemyJob(Unit worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<WoodResource>(this);
        return miningJob;
    }
}