using System;
using System.Collections.Generic;
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
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        playerState = GameObject.Find("PlayerState").GetComponent<PlayerState>();
        factory = GameObject.Find("Factory").GetComponent<Factory>();
        GameObject.Find("PlayerList").GetComponent<PlayerList>().players.Add(this);
    }

    public override void OnStartLocalPlayer()
    {
        playerState.player = this;
        gameState.player = this;

        //CreateTemporaryMainBuilding();
        CmdCreateUnit(transform.position, transform.position);
        if (isServer)
            CmdCreateResource(new Vector3(0, 0, 4));

    }

    public void Mine(int amount, GoldResource goldResource)
    {
        CmdMine(amount, goldResource.netId);
    }

    public void CreateTemporaryMainBuilding()
    {
        CmdCreateTempBuilding();
    }

    public void CreateMainBuilding(TemporaryBuilding tempBuilding)
    {
        CmdCreateMainBuilding(tempBuilding.netId);
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
    public void CmdCreateMainBuilding(NetworkInstanceId tempBuildingID)
    {
        var temporaryBuilding = NetworkServer.objects[tempBuildingID].GetComponent<TemporaryBuilding>();
        Building building = factory.CreateMainBuilding(temporaryBuilding, netId);
        NetworkServer.SpawnWithClientAuthority(building.gameObject, gameObject);
        gameState.RpcAddSelectable(building.netId);
        gameState.RpcRemoveSelectable(tempBuildingID);
        NetworkServer.Destroy(NetworkServer.objects[tempBuildingID].gameObject);
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
        NetworkServer.objects[unitId].GetComponent<Unit>().Arrived = value;
        if (value)
            gameState.RpcAddSelectable(unitId);
        else
            gameState.RpcRemoveSelectable(unitId);
    }
}