using System;

public class GoldResource : Resource
{
    public override string Name => "Gold mine";

    private static readonly float maxCapacity = 10000;

    protected override float MaxCapacity => maxCapacity;

    /// <summary>
    /// Substracts given amount of gold and gives it to the player.
    /// </summary>
    /// <param name="gathering">amount to gather</param>
    /// <param name="player">owner of the gatherer</param>
    /// <returns>true if the mine capacity is now 0</returns>
    public override bool Gather(float gathering, Player player)
    {
        PlayerState.Get(player.playerControllerId).Gold += Math.Min(gathering, capacity);
        return base.Gather(gathering, player);
    }

    public override Job GetEnemyJob(Unit worker)
    {
        if (miningJob == null)
            miningJob = new JobGather<GoldResource>(this);
        return miningJob;
    }
}
