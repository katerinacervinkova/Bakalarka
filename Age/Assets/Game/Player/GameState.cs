using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    public List<Unit> Units { get; private set; }
    public List<Building> Buildings { get; private set; }
    public List<Resource> Resources { get; private set; }
    public List<TemporaryBuilding> TemporaryBuildings { get; private set; }

    [SerializeField]
    private VisibilitySquares visibilitySquares;

    public VisibilitySquares VisibilitySquares => visibilitySquares;

    public override void OnStartClient()
    {
        Units = new List<Unit>();
        Buildings = new List<Building>();
        Resources = new List<Resource>();
        TemporaryBuildings = new List<TemporaryBuilding>();
        foreach (Resource resource in FindObjectsOfType<Resource>())
            Resources.Add(resource);
    }

    public void UpdateGraph(Bounds bounds)
    {
        var guo = new GraphUpdateObject(bounds)
        {
            modifyWalkability = true,
            updatePhysics = true
        };
        AstarPath.active?.UpdateGraphs(guo);
    }

    public T GetNearestResource<T>(T resource, Vector2 squareID) where T : Resource
    {
        return VisibilitySquares.NearestResource(resource, squareID);
    }

    public Selectable GetNearestTarget(Vector3 position, int maxDistance)
    {
        return ((IEnumerable<Selectable>)Units).Concat(Buildings).Where(s => s != null && !s.hasAuthority && Vector3.Distance(position, s.transform.position) < maxDistance).
            OrderBy(s => Vector3.Distance(position, s.transform.position)).FirstOrDefault();
    }

    private List<Vector2> AdjoiningSquares(Vector2 square)
    {
        return new List<Vector2>()
        {
            new Vector2(square.x - 1, square.y + 1),
            new Vector2(square.x - 1, square.y - 1),
            new Vector2(square.x - 1, square.y),
            new Vector2(square.x + 1, square.y + 1),
            new Vector2(square.x + 1, square.y - 1),
            new Vector2(square.x + 1, square.y),
            new Vector2(square.x, square.y + 1),
            new Vector2(square.x, square.y -1),
            square
        };
    }

    public void PositionChange(Unit unit)
    {
        var squarePosition = visibilitySquares.GetSquare(unit.transform.position);

        if (squarePosition != unit.SquareID)
        {
            visibilitySquares.RemoveFromSquare(unit.SquareID, unit);
            visibilitySquares.AddToSquare(squarePosition, unit);
            unit.SquareID = squarePosition;
        }
    }

    [ClientRpc]
    public void RpcPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        var temporaryBuilding = ClientScene.objects[tempBuildingId].GetComponent<TemporaryBuilding>();
        temporaryBuilding.OnPlaced(position);

        var squarePosition = visibilitySquares.GetSquare(temporaryBuilding.transform.position);
        visibilitySquares.AddToSquare(squarePosition, temporaryBuilding);
        temporaryBuilding.SquareID = squarePosition;

        var guo = new GraphUpdateObject(temporaryBuilding.GetComponent<Collider>().bounds)
        {
            modifyWalkability = true,
            updatePhysics = true,
            setWalkability = false
        };
        AstarPath.active.UpdateGraphs(guo);
    }

    [ClientRpc]
    public void RpcEnterBuilding(NetworkInstanceId unitId)
    {
        ClientScene.objects[unitId].gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcExitBuildingDestination(NetworkInstanceId unitId, Vector3 position, Vector3 destination)
    {
        Unit unit = ExitBuilding(unitId, position);
        if (unit.hasAuthority)
            unit.SetJob(new JobGo(destination));
    }

    [ClientRpc]
    public void RpcExitBuilding(NetworkInstanceId unitId, Vector3 position)
    {
        Unit unit = ExitBuilding(unitId, position);
        unit.ResetJob();
    }

    private static Unit ExitBuilding(NetworkInstanceId unitId, Vector3 position)
    {
        Unit unit = ClientScene.objects[unitId].GetComponent<Unit>();
        unit.transform.position = position;
        unit.SetVisibility(true);
        unit.gameObject.SetActive(true);
        return unit;
    }

    [ClientRpc]
    public void RpcCreateBuilding(NetworkInstanceId tempBuildingID, BuildingEnum buildingType)
    {
        var tempBuilding = ClientScene.objects[tempBuildingID].GetComponent<TemporaryBuilding>();
        Building building;
        switch (buildingType)
        {
            case BuildingEnum.MainBuilding:
                building = tempBuilding.gameObject.AddComponent<MainBuilding>() as MainBuilding;
                break;
            case BuildingEnum.Library:
                building = tempBuilding.gameObject.AddComponent<Library>() as Library;
                break;
            case BuildingEnum.Barracks:
                building = tempBuilding.gameObject.AddComponent<Barracks>() as Barracks;
                break;
            case BuildingEnum.Infirmary:
                building = tempBuilding.gameObject.AddComponent<Infirmary>() as Infirmary;
                break;
            case BuildingEnum.House:
                building = tempBuilding.gameObject.AddComponent<House>() as House;
                break;
            case BuildingEnum.Mill:
                building = tempBuilding.gameObject.AddComponent<Mill>() as Mill;
                break;
            default:
                building = null;
                break;
        }
        building.healthBarOffset = tempBuilding.healthBarOffset;
        building.owner = tempBuilding.owner;
        building.size = tempBuilding.size;
        building.SetHealthBar(tempBuilding.TransferHealthBar(building));
        building.Init();

        var squarePosition = visibilitySquares.GetSquare(tempBuilding.transform.position);
        visibilitySquares.AddToSquare(squarePosition, building);
        visibilitySquares.RemoveFromSquare(squarePosition, tempBuilding);
        building.SquareID = squarePosition;

        if (PlayerState.Instance.SelectedObject == tempBuilding)
            PlayerState.Instance.Select(building);
        Destroy(tempBuilding);
    }

    [ClientRpc]
    public void RpcDestroyObject(Vector3 center, Vector3 size)
    {
        var bounds = new Bounds(center, size);
        var guo = new GraphUpdateObject(bounds)
        {
            modifyWalkability = true,
            updatePhysics = true,
            setWalkability = true
        };
        AstarPath.active.UpdateGraphs(guo);
    }
}