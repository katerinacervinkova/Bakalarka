using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class GameState : NetworkBehaviour {

    private static GameState instance;
    public static GameState Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameState>();
            return instance;
        }
    }

    public Player player;
    public NavMeshSurface navMeshSurface;

    public GridGraph gridGraph;

    private List<Unit> units;
    private List<Building> buildings;
    private List<Resource> resources;

    // možná?
    private List<TemporaryBuilding> temporaryBuildings;

    public override void OnStartClient()
    {
        units = new List<Unit>();
        buildings = new List<Building>();
        resources = new List<Resource>();
        temporaryBuildings = new List<TemporaryBuilding>();
        navMeshSurface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
        foreach (var player in FindObjectsOfType<Player>())
            if (player.hasAuthority)
                this.player = player;
    }

    public void AddSelectable(Selectable selectable)
    {
        gridGraph.Add(selectable);
    }

    public void RemoveSelectable(Selectable selectable)
    {
        gridGraph.Remove(selectable);
    }

    public Vector3 GetClosestDestination(Vector3 location)
    {
        return gridGraph.ClosestDestination(location);
    }
    public Vector3 GetClosestUnoccupiedDestination(Vector3 location)
    {
        return gridGraph.ClosestUnoccupiedDestination(location);
    }

    public bool IsOccupied(Vector3 location)
    {
        return gridGraph.IsOccupied(location);
    }

    public bool IsOccupied(TemporaryBuilding buildingToBuild)
    {
        return gridGraph.IsOccupied(buildingToBuild);
    }

    [ClientRpc]
    public void RpcAddSelectable(NetworkInstanceId selectableId)
    {
        AddSelectable(ClientScene.objects[selectableId].GetComponent<Selectable>());
    }

    [ClientRpc]
    public void RpcRemoveSelectable(NetworkInstanceId selectableId)
    {
        RemoveSelectable(ClientScene.objects[selectableId]?.GetComponent<Selectable>());
    }

    [ClientRpc]
    public void RpcPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        GameObject temporaryBuilding = ClientScene.objects[tempBuildingId].gameObject;
        temporaryBuilding.transform.position = position;
        temporaryBuilding.SetActive(true);
        temporaryBuilding.GetComponent<NavMeshObstacle>().enabled = true;
    }

    [ClientRpc]
    internal void RpcSetDestination(Vector3 destination, NetworkInstanceId unitId)
    {
        ClientScene.objects[unitId].GetComponent<Unit>().SetDestination(destination);
    }

    [ClientRpc]
    internal void RpcCreateBuilding(NetworkInstanceId tempBuildingID)
    {
        var tempBuilding = ClientScene.objects[tempBuildingID].GetComponent<TemporaryBuilding>();
        Building building = tempBuilding.gameObject.AddComponent<MainBuilding>() as MainBuilding;
        building.owner = tempBuilding.owner;
        building.Init();
        AddSelectable(building);
        Destroy(tempBuilding);
    }
}
