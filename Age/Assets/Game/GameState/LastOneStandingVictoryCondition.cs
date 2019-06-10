using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastOneStandingVictoryCondition : VictoryCondition
{
    public override string GetDescription()
    {
        return "";
    }

    public override bool PlayerMeetsConditions(Player player) => players.Count == 1 && players[0] == player;

    public override bool PlayerMeetsLosingConditions(Player player) => PlayerState.Get(player.playerControllerId).units.Count == 0;
}
