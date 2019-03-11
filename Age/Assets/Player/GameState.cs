using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameState : NetworkBehaviour {

    public Unit unitPrefab;
    public Player player;
    public BottomBar bottomBar;
    public Factory factory;
    public NavMeshSurface navMeshSurface;

    private GridGraph gridGraph;

    private List<Unit> units;
    private List<Building> buildings;
    private List<Resource> resources;

    public Selectable SelectedObject { get; set; }
    public TemporaryBuilding BuildingToBuild { get; private set; }

    public Text nameText;
    public Text selectedObjectText;
    public Text resourceText;

    // možná?
    private List<TemporaryBuilding> temporaryBuildings;

    public void MoveBuildingToBuild(Vector3 hitPoint)
    {
        // pokud je to misto zabrane, nepohne se
        // TODO
        BuildingToBuild.transform.position = gridGraph.ClosestDestination(hitPoint);
        if (gridGraph.IsOccupied(BuildingToBuild))
            return;
    }

    public void OnResourceChange()
    {
        player.DrawBottomBar(resourceText);
    }

    [SyncVar]
    public NetworkInstanceId playerId;

    private void Awake()
    {
        units = new List<Unit>();
        buildings = new List<Building>();
        resources = new List<Resource>();
        temporaryBuildings = new List<TemporaryBuilding>();
        gridGraph = GameObject.Find("Map").GetComponent<GridGraph>();

        nameText = GameObject.Find("Canvas/Panel/nameText").GetComponent<Text>();
        selectedObjectText = GameObject.Find("Canvas/Panel/selectableAttributesText").GetComponent<Text>();
        resourceText = GameObject.Find("Canvas/Panel/resourceText").GetComponent<Text>();
        factory = GameObject.Find("Factory").GetComponent<Factory>();
        bottomBar = GameObject.Find("Canvas/Panel").GetComponent<BottomBar>();
        navMeshSurface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
    }

    public override void OnStartClient()
    {
        player = ClientScene.objects[playerId].GetComponent<Player>();
    }

    public override void OnStartAuthority()
    {
        factory.gameState = this;
        GameObject inputManager = GameObject.Find("InputManager");
        inputManager.GetComponent<LeftMouseActivity>().gameState = this;
        inputManager.GetComponent<RightMouseActivity>().gameState = this;
        if (isServer)
            CmdCreateResource(new Vector3(0, 0, 4), netId);
    }


    internal void AddSelectable(Selectable selectable)
    {
        gridGraph.Add(selectable);
    }

    internal void RemoveSelectable(Selectable selectable)
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

    public void SetWorkerAndBuilding(TemporaryBuilding building)
    {
        BuildingToBuild = building;
    }

    public void PlaceBuilding()
    {
        if (gridGraph.IsOccupied(BuildingToBuild))
            return;
        BuildingToBuild.CmdPlaceBuilding(BuildingToBuild.transform.position);
        ((Commandable)SelectedObject).SetGoal(BuildingToBuild);
        BuildingToBuild = null;
    }

    public void OnStateChange(Selectable selectable)
    {
        if (SelectedObject == selectable)
        {
            selectable.DrawBottomBar(nameText, selectedObjectText);
        }
    }

    public void Select(Selectable selectable)
    {
        if (SelectedObject == selectable)
            return;
        if (SelectedObject != null)
            Deselect();
        SelectedObject = selectable;
        selectable.SetSelection(true, player, bottomBar);
        selectable.DrawBottomBar(nameText, selectedObjectText);
        SetUIActive(true);
    }

    public void Deselect()
    {
        SelectedObject.RemoveBottomBar(nameText, selectedObjectText);
        SelectedObject.SetSelection(false, player, bottomBar);
        SetUIActive(false);
        SelectedObject = null;
    }

    private void SetUIActive(bool active)
    {
        if (nameText)
            nameText.gameObject.SetActive(active);
        if (selectedObjectText)
            selectedObjectText.gameObject.SetActive(active);
    }

    public void SelectUnits(Predicate<Unit> predicate)
    {
        var u = player.units.FindAll(predicate);
        if (u.Count == 0)
            return;
        if (u.Count == 1)
            Select(u[0]);
        else
            Select(factory.CreateRegiment(player, u));
    }

    public void CreateTemporaryMainBuilding()
    {
        CmdCreateTempBuilding(netId);
        UpdateNavMesh();
    }

    public void CreateMainBuilding(TemporaryBuilding tempBuilding)
    {
        CmdCreateMainBuilding(tempBuilding.netId, netId);
        UpdateNavMesh();
    }

    public void CreateUnit(Building building)
    {
        building.AddScheduler(
            factory.CreateScheduler(() => CmdCreateUnit(building.SpawnPoint, building.DefaultDestination, netId), null)
         );
    }

    private void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    [Command]
    public void CmdCreateResource(Vector3 position, NetworkInstanceId stateId)
    {
        GameState gameState = NetworkServer.objects[stateId].GetComponent<GameState>();
        Resource gold = gameState.factory.CreateGold(gridGraph.ClosestUnoccupiedDestination(position));
        NetworkServer.Spawn(gold.gameObject);
    }

    [Command]
    public void CmdCreateUnit(Vector3 position, Vector3 destination, NetworkInstanceId stateId)
    {
        GameState gameState = NetworkServer.objects[stateId].GetComponent<GameState>();
        Unit unit = gameState.factory.CreateUnit(gridGraph.ClosestUnoccupiedDestination(position), gameState.playerId);
        NetworkServer.SpawnWithClientAuthority(unit.gameObject, gameState.player.gameObject);
        RpcAddUnit(unit.netId, stateId);
        if (destination != position)
            unit.SetJob(new JobGo(unit, destination));
    }

    [ClientRpc]
    private void RpcAddUnit(NetworkInstanceId unitId, NetworkInstanceId stateId)
    {
        ClientScene.objects[stateId].GetComponent<GameState>().player.units.Add(ClientScene.objects[unitId].GetComponent<Unit>());
    }

    [ClientRpc]
    private void RpcRemoveUnit(NetworkInstanceId unitId, NetworkInstanceId stateId)
    {
        ClientScene.objects[stateId].GetComponent<GameState>().player.units.Remove(ClientScene.objects[unitId].GetComponent<Unit>());
    }

    [Command]
    public void CmdCreateTempBuilding(NetworkInstanceId stateId)
    {
        GameState gameState = NetworkServer.objects[stateId].GetComponent<GameState>();
        var tempBuilding = gameState.factory.CreateTemporaryMainBuilding(gameState.playerId);
        NetworkServer.SpawnWithClientAuthority(tempBuilding.gameObject, gameState.player.gameObject);
        RpcAddTempBuilding(tempBuilding.netId, stateId);
        tempBuilding.RpcOnCreate();
    }

    [ClientRpc]
    private void RpcAddTempBuilding(NetworkInstanceId buildingId, NetworkInstanceId stateId)
    {
        ClientScene.objects[stateId].GetComponent<GameState>().player.temporaryBuildings.Add(ClientScene.objects[buildingId].GetComponent<TemporaryBuilding>());
    }

    [ClientRpc]
    private void RpcRemoveTempBuilding(NetworkInstanceId buildingId, NetworkInstanceId stateId)
    {
        ClientScene.objects[stateId].GetComponent<GameState>().player.temporaryBuildings.Remove(ClientScene.objects[buildingId].GetComponent<TemporaryBuilding>());
    }

    [Command]
    public void CmdCreateMainBuilding(NetworkInstanceId tempBuildingID, NetworkInstanceId stateId)
    {
        GameState gameState = NetworkServer.objects[stateId].GetComponent<GameState>();
        var temporaryBuilding = NetworkServer.objects[tempBuildingID].GetComponent<TemporaryBuilding>();
        Building building = gameState.factory.CreateMainBuilding(temporaryBuilding, gameState.playerId);
        NetworkServer.SpawnWithClientAuthority(building.gameObject, gameState.player.gameObject);
        RpcAddBuilding(building.netId, stateId);
        //RpcRemoveTempBuilding(tempBuildingID, stateId);
        NetworkServer.Destroy(NetworkServer.objects[tempBuildingID].gameObject);
    }

    [ClientRpc]
    private void RpcAddBuilding(NetworkInstanceId buildingId, NetworkInstanceId stateId)
    {
        ClientScene.objects[stateId].GetComponent<GameState>().player.buildings.Add(ClientScene.objects[buildingId].GetComponent<Building>());
    }

    [ClientRpc]
    private void RpcRemoveBuilding(NetworkInstanceId buildingId, NetworkInstanceId stateId)
    {
        ClientScene.objects[stateId].GetComponent<GameState>().player.buildings.Remove(ClientScene.objects[buildingId].GetComponent<Building>());
    }
}
