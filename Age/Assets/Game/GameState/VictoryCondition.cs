using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public abstract class VictoryCondition : NetworkBehaviour
{
    [SyncVar]
    protected int playerCount;

    public List<Player> players;

    public bool InGame = false;

    public override void OnStartClient()
    {
        FindObjectsOfType<Player>().ToList();
        players.ForEach(p => {
            p.victoryCondition = this;
        });
    }

    public override void OnStartAuthority() => CmdPlayerCount();

    [Command]
    private void CmdPlayerCount() => playerCount = FindObjectOfType<CustomLobbyManager>().playerCount;

    public virtual bool PlayerMeetsConditions(Player player)
    {
        if (InGame)
        {
            var ps = players.Where(p => p != null && p.InGame);
            if (ps.Count() == 1 && ps.First() == player)
                return true;
        }
        else if (playerCount == players.Count && players.TrueForAll(p => p.InGame))
            InGame = true;
        return false;
    }

    public abstract bool PlayerMeetsLosingConditions(Player player);
}