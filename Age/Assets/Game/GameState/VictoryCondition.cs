using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public abstract class VictoryCondition : NetworkBehaviour
{
    protected List<Player> players = new List<Player>();
    [SerializeField]
    private GameObject winningText;
    [SerializeField]
    private GameObject losingText;

    public override void OnStartClient()
    {
        players = FindObjectsOfType<Player>().ToList();
        players.ForEach(p => {
            p.victoryCondition = this;
            p.winningText = winningText;
            p.losingText = losingText;
        });
    }

    [Command]
    public void CmdRemove(NetworkInstanceId playerId)
    {
        RpcRemove(playerId);
    }

    [ClientRpc]
    private void RpcRemove(NetworkInstanceId playerId)
    {
        players.Remove(ClientScene.objects[playerId].GetComponent<Player>());
    }

    private void Update()
    {
        players.ForEach(
            p => {
                if (PlayerMeetsConditions(p))
                {
                    p.Win();
                    players.ForEach(pl => { if (p != pl) pl.Lose(); });
                }
            });
    }

    public abstract string GetDescription();
    public abstract bool PlayerMeetsConditions(Player player);
    public abstract bool PlayerMeetsLosingConditions(Player player);
}