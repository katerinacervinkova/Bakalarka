using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameState : NetworkBehaviour {

    public Unit unitPrefab;
    public Player player;
    public GridGraph gridGraph;
    public BottomBar bottomBar;
    public Factory factory;
    public NavMeshSurface navMeshSurface;

    private List<Unit> units;
    private List<Building> buildings;
    private List<Resource> resources;

    public Selectable SelectedObject { get; set; }
    public TemporaryBuilding BuildingToBuild { get; private set; }

    public Text nameText;
    public Text selectedObjectText;

    public Commandable Worker { get; private set; }

    // možná?
    private List<TemporaryBuilding> temporaryBuildings;

    private void Awake()
    {
        nameText = GameObject.Find("Canvas/Panel/nameText").GetComponent<Text>();
        selectedObjectText = GameObject.Find("Canvas/Panel/selectableAttributesText").GetComponent<Text>();

        player = gameObject.GetComponent<Player>();
        factory = GameObject.Find("Factory").GetComponent<Factory>();
        gridGraph = GameObject.Find("Map").GetComponent<GridGraph>();
        bottomBar = GameObject.Find("Panel").GetComponent<BottomBar>();
        navMeshSurface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();

        units = new List<Unit>();
        buildings = new List<Building>();
        resources = new List<Resource>();

        temporaryBuildings = new List<TemporaryBuilding>();
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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (hasAuthority)
        {
            factory.gameState = this;
            GameObject inputManager = GameObject.Find("InputManager");
            inputManager.GetComponent<LeftMouseActivity>().gameState = this;
            inputManager.GetComponent<RightMouseActivity>().gameState = this;
        }
    }

    public void SetWorkerAndBuilding(TemporaryBuilding building, Commandable worker)
    {
        BuildingToBuild = building;
        Worker = worker;
    }

    public void PlaceBuilding()
    {
        BuildingToBuild.CmdPlaceBuilding(BuildingToBuild.transform.position);
        Worker.SetGoal(BuildingToBuild);
        BuildingToBuild = null;
        Worker = null;
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
        SelectedObject.SetSelection(false, player, bottomBar);
        SelectedObject.RemoveBottomBar(nameText, selectedObjectText);
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

    public void CreateTemporaryMainBuilding(Commandable commandable)
    {
        CmdCreateTempBuilding(commandable.netId);
    }

    public void CreateUnit(Building building)
    {
        building.AddScheduler(
            factory.CreateScheduler(() => CmdCreateUnit(building.SpawnPoint, building.DefaultDestination), null)
         );
    }

    [Command]
    public void CmdCreateMainBuilding(NetworkInstanceId tempBuildingID)
    {
        var temporaryBuilding = NetworkServer.objects[tempBuildingID].GetComponent<TemporaryBuilding>();
        Building building = factory.CreateMainBuilding(temporaryBuilding);
        NetworkServer.SpawnWithClientAuthority(building.gameObject, gameObject);
        player.buildings.Add(building);
        NetworkServer.Destroy(NetworkServer.objects[tempBuildingID].gameObject);
    }

    private void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    [Command]
    public void CmdCreateUnit(Vector3 position, Vector3 destination)
    {
        Unit unit = factory.CreateUnit(gridGraph.ClosestUnoccupiedDestination(position));
        unit.playerID = player.netId;
        player.units.Add(unit);
        units.Add(unit);
        NetworkServer.SpawnWithClientAuthority(unit.gameObject, gameObject);
        unit.CmdChangeArrived(true);
        if (destination != position)
            unit.SetGo(destination);
    }

    [Command]
    private void CmdCreateTempBuilding(NetworkInstanceId workerID)
    {
        var tempBuilding = factory.CreateTemporaryMainBuilding(player);
        NetworkServer.SpawnWithClientAuthority(tempBuilding.gameObject, gameObject);
        tempBuilding.RpcOnCreate(workerID);
        UpdateNavMesh();
    }
}
