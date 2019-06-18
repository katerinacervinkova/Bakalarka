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

    public GameObject errorCanvas;

    public List<Unit> Units { get; private set; } = new List<Unit>();
    public List<Building> Buildings { get; private set; } = new List<Building>();
    public List<Resource> Resources { get; private set; } = new List<Resource>();
    public List<TemporaryBuilding> TemporaryBuildings { get; private set; } = new List<TemporaryBuilding>();

    private VisibilitySquares[] squaresInstances = new VisibilitySquares[6];
    public VisibilitySquares GetSquares(int i = 0) => squaresInstances[i];

    public void SetVisibilitySquares(short playerId, VisibilitySquares visibilitySquares)
    {
        squaresInstances[playerId] = visibilitySquares;
        visibilitySquares.playerId = playerId;
    }

    private void Start()
    {
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

    public List<Unit> VisibleEnemyUnits(int playerId) => GetSquares(playerId).VisibleEnemyUnits();
    public List<Building> VisibleEnemyBuildings(int playerId) => GetSquares(playerId).VisibleEnemyBuildings();
    public List<TemporaryBuilding> VisibleEnemyTemporaryBuildings(int playerId) => GetSquares(playerId).VisibleEnemyTemporaryBuildings();
    public List<Resource> VisibleResources(int playerId) => GetSquares(playerId).VisibleResources();

    public T GetClosestResource<T>(Vector2 squareID, T resource) where T : Resource => GetSquares().ClosestVisibleResource(resource, squareID);
    public T ClosestVisibleResource<T>(Vector3 destination, int playerId) where T : Resource => GetSquares(playerId).ClosestGloballyVisibleResource<T>(SquarePosition(destination));

    public Selectable ClosestVisibleTarget(Vector3 position, int playerId) => GetSquares(playerId).ClosestVisibleTarget(SquarePosition(position));

    public Vector3 GetClosestFreePosition(Vector3 position, int playerId) => GetSquares(playerId).GetClosestFreePosition(position);

    public void PositionChange(Unit unit)
    {
        Vector2 squarePosition = SquarePosition(unit.transform.position);

        if (squarePosition != unit.SquareID && !float.IsPositiveInfinity(squarePosition.x))
        {
            ForEachSquare(s =>
            {
                s.RemoveFromSquare(unit.SquareID, unit);
                s.AddToSquare(squarePosition, unit);
            });
            unit.SquareID = squarePosition;
        }
    }

    private Vector2 SquarePosition(Vector3 position)
    {
        if (GetSquares() == null)
            return Vector2.positiveInfinity;
        return GetSquares().GetSquare(position);
    }

    public void RemoveFromSquare(Vector2 squareId, TemporaryBuilding selectable) => ForEachSquare(s => s.RemoveFromSquare(squareId, selectable));

    public void RemoveFromSquare(Vector2 squareId, Building selectable) => ForEachSquare(s => s.RemoveFromSquare(squareId, selectable));
    public void RemoveFromSquare(Vector2 squareId, Resource selectable) => ForEachSquare(s => s.RemoveFromSquare(squareId, selectable));

    private void ForEachSquare(Action<VisibilitySquares> action)
    {
        foreach (var visibilitySquares in squaresInstances)
            if (visibilitySquares != null)
                action.Invoke(visibilitySquares);
    }

    [ClientRpc]
    public void RpcPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        var temporaryBuilding = ClientScene.objects[tempBuildingId].GetComponent<TemporaryBuilding>();
        temporaryBuilding.OnPlaced(position);

        var squarePosition = SquarePosition(temporaryBuilding.transform.position);
        ForEachSquare(s => s.AddToSquare(squarePosition, temporaryBuilding));
        temporaryBuilding.SquareID = squarePosition;

        UpdateGraph(temporaryBuilding.GetComponent<Collider>().bounds);
    }

    [ClientRpc]
    public void RpcEnterBuilding(NetworkInstanceId unitId)
    {
        ClientScene.objects[unitId].gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcExitBuildingDestination(NetworkInstanceId unitId, Vector3 position, Vector3 destination)
    {
        Unit unit = ClientScene.objects[unitId].GetComponent<Unit>();
        unit.ExitBuilding(position);
        if (unit.hasAuthority)
            unit.SetJob(new JobGo(destination));
    }

    [ClientRpc]
    public void RpcExitBuilding(NetworkInstanceId unitId, Vector3 position)
    {
        Unit unit = ClientScene.objects[unitId].GetComponent<Unit>();
        unit.ExitBuilding(position);
        unit.ResetJob();
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
        building.playerId = tempBuilding.playerId;
        building.SetHealthBar(tempBuilding.TransferHealthBar(building));
        building.Init();

        var squarePosition = tempBuilding.SquareID;
        ForEachSquare(s =>
        {
            s.AddToSquare(squarePosition, building);
            s.RemoveFromSquare(squarePosition, tempBuilding);
        });
        building.SquareID = squarePosition;

        foreach (var playerState in PlayerState.GetAll())
            if (playerState != null && playerState.SelectedObject == tempBuilding)
                playerState.Select(building);
        Destroy(tempBuilding);
    }

    [ClientRpc]
    public void RpcAttack(NetworkInstanceId attackerId, NetworkInstanceId targetId)
    {
        NetworkIdentity targetIdentity;
        ClientScene.objects.TryGetValue(targetId, out targetIdentity);
        if (targetIdentity != null)
        {
            Selectable target = targetIdentity.GetComponent<Selectable>();
            if (target.hasAuthority)
            {
                NetworkIdentity attackerIdentity;
                ClientScene.objects.TryGetValue(attackerId, out attackerIdentity);
                if (attackerIdentity != null)
                    target.DealAttack(attackerIdentity.GetComponent<Selectable>());
            }
        }
    }

    [ClientRpc]
    public void RpcDestroyObject(Vector3 center, Vector3 size)
    {
        UpdateGraph(new Bounds(center, size));
    }
}