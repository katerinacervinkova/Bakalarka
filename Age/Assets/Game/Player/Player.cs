using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    [SyncVar]
    public string Name;
    [SyncVar]
    public Color color;

    public Factory factory;

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.parent.position = transform.position;
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.player = this;
            PlayerState.Instance.OnResourceChange();
        }
        if (GameState.Instance != null)
            GameState.Instance.player = this;
    }

    public bool CreateInitialUnit()
    {
        if (!hasAuthority)
            return false;
        if (connectionToClient == null || connectionToClient.isReady)
        {
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
        Resource gold = factory.CreateGold(GameState.Instance.GetClosestUnoccupiedDestination(position));
        gold.playerID = netId;
        NetworkServer.Spawn(gold.gameObject);
        GameState.Instance.RpcAddSelectable(gold.netId);
    }

    [Command]
    public void CmdCreateUnit(Vector3 position, Vector3 destination)
    {
        Unit unit = factory.CreateUnit(GameState.Instance.GetClosestUnoccupiedDestination(position), netId);
        NetworkServer.SpawnWithClientAuthority(unit.gameObject, gameObject);
        if (destination != position)
            unit.SetJob(new JobGo(unit, destination));
        else
            GameState.Instance.RpcAddSelectable(unit.netId);

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
        GameState.Instance.RpcCreateBuilding(tempBuildingID);
    }

    [Command]
    public void CmdSetDestination(Vector3 destination, NetworkInstanceId unitId)
    {
        NetworkServer.objects[unitId].GetComponent<Unit>().SetDestination(destination);
        GameState.Instance.RpcSetDestination(destination, unitId);
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
        GameState.Instance.RpcPlaceBuilding(position, tempBuildingId);
        GameState.Instance.RpcAddSelectable(tempBuildingId);
    }

    [Command]
    public void CmdUnitArrived(bool value, NetworkInstanceId unitId)
    {
        Unit unit = NetworkServer.objects[unitId].GetComponent<Unit>();
        unit.Arrived = value;
        if (value)
            GameState.Instance.RpcAddSelectable(unitId);
        else
            GameState.Instance.RpcRemoveSelectable(unitId);
    }
}