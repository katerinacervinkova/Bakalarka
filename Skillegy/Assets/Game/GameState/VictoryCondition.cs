using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

/// <summary>
/// Base for any victory condition that can be used in this game
/// </summary>
public abstract class VictoryCondition : NetworkBehaviour
{
    [SyncVar]
    protected int playerCount;

    public List<Player> players;

    // true if game has already started.
    public bool InGame = false;

    public override void OnStartClient()
    {
        FindObjectsOfType<Player>().ToList();
        players.ForEach(p => {
            p.victoryCondition = this;
        });
    }

    /// <summary>
    /// Gets the player count from the lobby manager.
    /// </summary>
    public override void OnStartAuthority() => CmdPlayerCount();

    [Command]
    private void CmdPlayerCount() => playerCount = FindObjectOfType<CustomLobbyManager>().playerCount;

    public virtual bool PlayerMeetsConditions(Player player)
    {
        if (InGame)
        {
            // player wins if he is the only one left
            var ps = players.Where(p => p != null && p.InGame);
            if (ps.Count() == 1 && ps.First() == player)
                return true;
        }
        // if all players are initialized and ready, game has started
        else if (playerCount > 0 && playerCount == players.Count && players.TrueForAll(p => p.InGame))
            InGame = true;
        return false;
    }

    public abstract bool PlayerMeetsLosingConditions(Player player);
}