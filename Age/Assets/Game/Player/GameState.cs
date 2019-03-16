using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class GameState : NetworkBehaviour {

    public Player player;
    public PlayerState playerState;
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
        playerState = GameObject.Find("PlayerState").GetComponent<PlayerState>();
        navMeshSurface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
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
        gridGraph.Add(ClientScene.objects[selectableId].GetComponent<Selectable>());
    }

    [ClientRpc]
    public void RpcRemoveSelectable(NetworkInstanceId selectableId)
    {
        gridGraph.Remove(ClientScene.objects[selectableId]?.GetComponent<Selectable>());
    }

    [ClientRpc]
    public void RpcPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        GameObject temporaryBuilding = ClientScene.objects[tempBuildingId].gameObject;
        temporaryBuilding.transform.position = position;
        temporaryBuilding.SetActive(true);
        temporaryBuilding.GetComponent<NavMeshObstacle>().enabled = true;
    }
}
