
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public string Name;

    [SyncVar]
    public Color color;
    public List<Unit> units;
    public List<Building> buildings;
    public List<TemporaryBuilding> temporaryBuildings;

    public GameObject gameStatePrefab;


    public override void OnStartLocalPlayer()
    {
        CmdCreateState(netId);
    }

    [Command]
    private void CmdCreateState(NetworkInstanceId networkId)
    {
        GameState gameState = Instantiate(gameStatePrefab, transform.position, Quaternion.identity).GetComponent<GameState>();
        gameState.playerId = netId;
        NetworkServer.SpawnWithClientAuthority(gameState.gameObject, gameObject);
        
    }
}