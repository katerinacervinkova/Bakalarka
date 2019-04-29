using Pathfinding;
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


    public void Gather(float amount, Resource resource)
    {
        CmdGather(amount, resource.netId);
    }

    public void CreateTemporaryMainBuilding()
    {
        CmdCreateTempBuilding();
    }

    public void CreateUnit(Building building)
    {
        CmdCreateUnit(building.SpawnPoint, building.DefaultDestination);
    }

    public void PlaceBuilding(TemporaryBuilding temporaryBuilding)
    {
        var position = temporaryBuilding.transform.position;
        CmdPlaceBuilding(new Vector3((float)Math.Round(position.x), position.y, (float)Math.Round(position.z)), temporaryBuilding.netId);
    }

    [Command]
    public void CmdCreateUnit(Vector3 position, Vector3 destination)
    {
        NNConstraint nodeConstraint = new NNConstraint
        {
            constrainWalkability = true,
            walkable = true
        };
        NNInfo nodeInfo = AstarPath.active.GetNearest(position, nodeConstraint);
        Unit unit = factory.CreateUnit(nodeInfo.position, netId);
        NetworkServer.SpawnWithClientAuthority(unit.gameObject, gameObject);
        if (destination != position)
            unit.SetJob(new JobGo(destination));
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
    public void CmdGather(float amount, NetworkInstanceId resourceId)
    {
        if (!NetworkServer.objects.ContainsKey(resourceId))
            return;
        Resource resource = NetworkServer.objects[resourceId].GetComponent<Resource>();
        resource.capacity -= amount;
        if (resource.capacity <= 0)
            NetworkServer.Destroy(resource.gameObject);
    }

    [Command]
    public void CmdPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        TemporaryBuilding temporaryBuilding = NetworkServer.objects[tempBuildingId].GetComponent<TemporaryBuilding>();
        temporaryBuilding.transform.position = position;
        temporaryBuilding.placed = true;
        GameState.Instance.RpcPlaceBuilding(position, tempBuildingId);
    }

    [Command]
    public void CmdOnPathCompleted(NetworkInstanceId unitId)
    {

    }
}