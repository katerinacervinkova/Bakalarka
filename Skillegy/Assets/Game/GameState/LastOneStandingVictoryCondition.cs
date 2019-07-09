public class LastOneStandingVictoryCondition : VictoryCondition
{
    // player loses if he has no units
    public override bool PlayerMeetsLosingConditions(Player player) => PlayerState.Get(player.playerControllerId).units.Count == 0;
}
