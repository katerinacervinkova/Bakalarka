using System.Linq;

public class LastOneStandingVictoryCondition : VictoryCondition
{
    public override bool PlayerMeetsLosingConditions(Player player) => PlayerState.Get(player.playerControllerId).units.Count == 0;
}
