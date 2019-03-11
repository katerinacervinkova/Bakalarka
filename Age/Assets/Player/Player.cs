using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    public List<Player> players = new List<Player>();

    [SyncVar]
    public string Name;

    [SyncVar]
    public Color color;

    [SyncVar(hook = "OnGoldChange")]
    public int gold = 100;

    public List<Unit> units;
    public List<Building> buildings;
    public List<TemporaryBuilding> temporaryBuildings;

    public GameObject gameStatePrefab;
    public GameState gameState;

    public override void OnStartClient()
    {
        GameObject.Find("PlayerList").GetComponent<PlayerList>().players.Add(this);
    }

    public override void OnStartLocalPlayer()
    {
        CmdCreateState(netId);
    }

    private void OnGoldChange(int newGold)
    {
        gold = newGold;
        gameState.OnResourceChange();
    }

    [Command]
    private void CmdCreateState(NetworkInstanceId networkId)
    {
        GameState gameState = Instantiate(gameStatePrefab, transform.position, Quaternion.identity).GetComponent<GameState>();
        gameState.playerId = networkId;
        NetworkServer.SpawnWithClientAuthority(gameState.gameObject, NetworkServer.objects[networkId].gameObject);
        RpcStoreGameState(gameState.netId);
    }

    public void DrawBottomBar(Text resourceText)
    {
        resourceText.text = "Gold: " + gold;
    }

    [ClientRpc]
    private void RpcStoreGameState(NetworkInstanceId stateId)
    {
        GameState gs = ClientScene.objects[stateId].GetComponent<GameState>();
        if (gs.hasAuthority)
        {
            gameState = gs;
            foreach (var selectable in FindObjectsOfType(typeof(Selectable)) as Selectable[])
                selectable.InitGameState();
            gameState.CmdCreateUnit(transform.position, transform.position, gameState.netId);
            CmdInitResources(netId);
        }
    }

    [Command]
    private void CmdInitResources(NetworkInstanceId playerId)
    {
        NetworkServer.objects[playerId].GetComponent<Player>().gold = 100;
    }

    public bool PayGold(int amount)
    {
        if (gold < amount)
            return false;
        CmdPay(amount, netId);
        return true;
    }

    [Command]
    private void CmdPay(int amount, NetworkInstanceId playerId)
    {
        NetworkServer.objects[playerId].GetComponent<Player>().gold -= amount;
    }
}