using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    [SyncVar]
    public string Name;
    [SyncVar]
    public Color color;

    public GameState gameState;
    public PlayerState playerState;

    public Factory factory;
    public override void OnStartClient()
    {
        gameState = GameObject.Find("GameState")?.GetComponent<GameState>();
        playerState = GameObject.Find("PlayerState")?.GetComponent<PlayerState>();
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.position += transform.position;
        if (playerState != null)
        {
            playerState.player = this;
            playerState.OnResourceChange();
        }
        if (gameState != null)
            gameState.player = this;
    }
     
    public void Register(PlayerState newPlayerState)
    {
        if (playerState != null)
            return;
        playerState = newPlayerState;
        if (hasAuthority)
        {
            playerState.player = this;
            playerState.OnResourceChange();
        }
    }

    public void Register(GameState newGameState)
    {
        if (gameState != null)
            return;
        gameState = newGameState;
        if (hasAuthority)
            gameState.player = this;
    }

    public bool CreateInitialUnit()
    {
        if (!hasAuthority || gameState == null || playerState == null || factory == null)
            return false;
        if (connectionToClient == null || connectionToClient.isReady)
        {
            if (isServer)
                CmdCreateResource(new Vector3(0, 0, 4));
            CmdCreateUnit(transform.position, transform.position);
            return true;
        }
        return false;
    }


    public void Mine(int amount, Resource resource)
    {
        CmdMine(amount, resource.netId);
    }

    public void CreateTemporaryMainBuilding()
    {
        CmdCreateTempBuilding();
    }

    public void CreateUnit(Building building)
    {
        building.AddScheduler(
            factory.CreateScheduler(() => CmdCreateUnit(building.SpawnPoint, building.DefaultDestination), null)
         );
    }

    public void PlaceBuilding(TemporaryBuilding temporaryBuilding)
    {
        CmdPlaceBuilding(temporaryBuilding.transform.position, temporaryBuilding.netId);
    }

    [Command]
    public void CmdCreateResource(Vector3 position)
    {
        Resource gold = factory.CreateGold(gameState.GetClosestUnoccupiedDestination(position));
        gold.playerID = netId;
        NetworkServer.Spawn(gold.gameObject);
        gameState.RpcAddSelectable(gold.netId);
    }

    [Command]
    public void CmdCreateUnit(Vector3 position, Vector3 destination)
    {
        Unit unit = factory.CreateUnit(gameState.GetClosestUnoccupiedDestination(position), netId);
        NetworkServer.SpawnWithClientAuthority(unit.gameObject, gameObject);
        if (destination != position)
            unit.SetJob(new JobGo(unit, destination));
        else
            gameState.RpcAddSelectable(unit.netId);

    }

    [Command]
    public void CmdCreateTempBuilding()
    {
        var tempBuilding = factory.CreateTemporaryMainBuilding(netId);
        NetworkServer.SpawnWithClientAuthority(tempBuilding.gameObject, gameObject);
        tempBuilding.RpcOnCreate();
    }

    [Command]
    public void CmdCreateBuilding(NetworkInstanceId tempBuildingID)
    {
        gameState.RpcCreateBuilding(tempBuildingID);
    }

    [Command]
    public void CmdSetDestination(Vector3 destination, NetworkInstanceId unitId)
    {
        NetworkServer.objects[unitId].GetComponent<Unit>().SetDestination(destination);
        gameState.RpcSetDestination(destination, unitId);
    }

    [Command]
    public void CmdMine(int amount, NetworkInstanceId resourceId)
    {
        Resource resource = NetworkServer.objects[resourceId].GetComponent<Resource>();
        resource.capacity -= amount;
        if (resource.capacity <= 0)
        {
            NetworkServer.Destroy(resource.gameObject);
        }
    }

    [Command]
    public void CmdPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        TemporaryBuilding temporaryBuilding = NetworkServer.objects[tempBuildingId].GetComponent<TemporaryBuilding>();
        temporaryBuilding.transform.position = position;
        temporaryBuilding.placed = true;
        gameState.RpcPlaceBuilding(position, tempBuildingId);
        gameState.RpcAddSelectable(tempBuildingId);
    }

    [Command]
    public void CmdUnitArrived(bool value, NetworkInstanceId unitId)
    {
        Unit unit = NetworkServer.objects[unitId].GetComponent<Unit>();
        unit.Arrived = value;
        if (value)
            gameState.RpcAddSelectable(unitId);
        else
            gameState.RpcRemoveSelectable(unitId);
    }
}