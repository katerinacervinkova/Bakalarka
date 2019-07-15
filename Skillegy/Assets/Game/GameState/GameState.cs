using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class used for getting information about game states and for synchronizing game state with all clients.
/// </summary>
public class GameState : NetworkBehaviour {

    // GameState is a singleton to make it easier to access
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

    public readonly int MapSize = 200;

    public List<Unit> Units { get; private set; } = new List<Unit>();
    public List<Building> Buildings { get; private set; } = new List<Building>();
    public List<Resource> Resources { get; private set; } = new List<Resource>();
    public List<TemporaryBuilding> TemporaryBuildings { get; private set; } = new List<TemporaryBuilding>();

    // one instance for each player using this client
    private VisibilitySquares[] squaresInstances = new VisibilitySquares[6];
    /// <summary>
    /// Gets the corresponding VisibilitySquares.
    /// </summary>
    /// <param name="playerControllerId">ID of the controller the player is using</param>
    /// <returns>orresponding VisibilitySquares</returns>
    public VisibilitySquares GetSquares(int playerControllerId = 0) => squaresInstances[playerControllerId];

    /// <summary>
    /// Inits the visibility squares.
    /// </summary>
    /// <param name="playerId">ID of the controller the player is using</param>
    /// <param name="visibilitySquares">visibility squares to be initialized</param>
    public void SetVisibilitySquares(short playerId, VisibilitySquares visibilitySquares)
    {
        squaresInstances[playerId] = visibilitySquares;
        visibilitySquares.playerId = playerId;
    }

    /// <summary>
    /// Register all resources.
    /// </summary>
    private void Start()
    {
        foreach (Resource resource in FindObjectsOfType<Resource>())
            Resources.Add(resource);
    }

    /// <summary>
    /// Updates the pathfinding graph according to the given bounds.
    /// </summary>
    /// <param name="bounds">bounds according to which the graph is to be updated</param>
    public void UpdateGraph(Bounds bounds)
    {
        var guo = new GraphUpdateObject(bounds)
        {
            modifyWalkability = true,
            updatePhysics = true,
            setWalkability = false,
        };
        AstarPath.active?.UpdateGraphs(guo);
    }

    public List<Unit> VisibleEnemyUnits(int playerId) => GetSquares(playerId).VisibleEnemyUnits();
    public List<Building> VisibleEnemyBuildings(int playerId) => GetSquares(playerId).VisibleEnemyBuildings();
    public List<TemporaryBuilding> VisibleEnemyTemporaryBuildings(int playerId) => GetSquares(playerId).VisibleEnemyTemporaryBuildings();
    public List<Resource> VisibleResources(int playerId) => GetSquares(playerId).VisibleResources();

    /// <summary>
    /// Shows the error message. Called when server disconnects.
    /// </summary>
    public void OnClientDisconnect()
    {
        if (PlayerState.Get().player.InGame)
            errorCanvas.SetActive(true);
    }

    public T GetClosestResource<T>(Vector2 squareID, T resource) where T : Resource => GetSquares().ClosestVisibleResource(resource, squareID);
    public T ClosestVisibleResource<T>(Vector3 destination, int playerId) where T : Resource => GetSquares(playerId).ClosestVisibleResource<T>(SquareId(destination));
    public Selectable ClosestVisibleTarget(Vector3 position, int playerId) => GetSquares(playerId).ClosestVisibleTarget(SquareId(position));

    public Vector3 GetRandomDestination(Vector3 position, int distance) => position + new Vector3(UnityEngine.Random.value - 0.5f, 0, UnityEngine.Random.value - 0.5f) * distance * 2;
    public Vector3 GetRandomDestination() => GetRandomDestination(new Vector3(), MapSize);


    /// <summary>
    /// Updates unit position in visibility square based on its position change.
    /// </summary>
    /// <param name="unit">unit to be updated</param>
    public void PositionChange(Unit unit)
    {
        Vector2 squareId = SquareId(unit.transform.position);

        if (squareId != unit.SquareID && !float.IsPositiveInfinity(squareId.x))
        {
            ForEachSquare(s =>
            {
                s.RemoveFromSquare(unit.SquareID, unit);
                s.AddToSquare(squareId, unit);
            });
            unit.SquareID = squareId;
        }
    }

    /// <summary>
    /// Returns ID of the visibility square for given position.
    /// </summary>
    /// <param name="position">position in the real world</param>
    /// <returns>ID of the visibility square for given position</returns>
    private Vector2 SquareId(Vector3 position)
    {
        if (GetSquares() == null)
            return Vector2.positiveInfinity;
        return GetSquares().GetSquare(position);
    }

    public void RemoveFromSquare(Vector2 squareId, TemporaryBuilding selectable) => ForEachSquare(s => s.RemoveFromSquare(squareId, selectable));
    public void RemoveFromSquare(Vector2 squareId, Building selectable) => ForEachSquare(s => s.RemoveFromSquare(squareId, selectable));
    public void RemoveFromSquare(Vector2 squareId, Resource selectable) => ForEachSquare(s => s.RemoveFromSquare(squareId, selectable));


    /// <summary>
    /// Does given action for all instances of VisibilitySquares.
    /// </summary>
    /// <param name="action">action to be performed</param>
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

        // updates the visibility squares
        var squarePosition = SquareId(temporaryBuilding.transform.position);
        ForEachSquare(s => s.AddToSquare(squarePosition, temporaryBuilding));
        temporaryBuilding.SquareID = squarePosition;

        UpdateGraph(temporaryBuilding.GetComponent<Collider>().bounds);
    }

    /// <summary>
    /// Synchronizes unit entering the building by letting the unit disappear.
    /// </summary>
    /// <param name="unitId">netId of the unit</param>
    [ClientRpc]
    public void RpcEnterBuilding(NetworkInstanceId unitId)
    {
        ClientScene.objects[unitId].gameObject.SetActive(false);
    }

    /// <summary>
    /// Synchronizes unit by letting it reappear in given position and sending it to given destination
    /// </summary>
    /// <param name="unitId">netId of the unit</param>
    /// <param name="position">position for the unit to appear</param>
    /// <param name="destination">position for the unit to go</param>
    [ClientRpc]
    public void RpcExitBuildingDestination(NetworkInstanceId unitId, Vector3 position, Vector3 destination)
    {
        Unit unit = ClientScene.objects[unitId].GetComponent<Unit>();
        unit.ExitBuilding(position);
        if (unit.hasAuthority)
            unit.SetJob(new JobGo(destination));
    }

    /// <summary>
    /// Synchronizes unit by letting it reappear in given position.
    /// </summary>
    /// <param name="unitId">netId of the unit</param>
    /// <param name="position">position for the unit to appear</param>
    [ClientRpc]
    public void RpcExitBuilding(NetworkInstanceId unitId, Vector3 position)
    {
        Unit unit = ClientScene.objects[unitId].GetComponent<Unit>();
        unit.ExitBuilding(position);
        unit.ResetJob();
    }

    /// <summary>
    /// Turn given temporary building into building of given type.
    /// </summary>
    /// <param name="tempBuildingID">temporary building to turn into building</param>
    /// <param name="buildingType">type of the building</param>
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
            case BuildingEnum.Sawmill:
                building = tempBuilding.gameObject.AddComponent<Sawmill>() as Sawmill;
                break;
            case BuildingEnum.Bank:
                building = tempBuilding.gameObject.AddComponent<Bank>() as Bank;
                break;
            default:
                building = null;
                break;
        }

        //inits the building
        building.healthBarOffset = tempBuilding.healthBarOffset;
        building.owner = tempBuilding.owner;
        building.size = tempBuilding.size;
        building.playerId = tempBuilding.playerId;
        building.MaxHealth = tempBuilding.MaxHealth;
        building.SetHealthBar(tempBuilding.TransferHealthBar(building));
        building.Init();

        // updates the visibility squares
        var squarePosition = tempBuilding.SquareID;
        ForEachSquare(s =>
        {
            s.AddToSquare(squarePosition, building);
            s.RemoveFromSquare(squarePosition, tempBuilding);
        });
        building.SquareID = squarePosition;

        // select the building if the temporary building was selected
        foreach (var playerState in PlayerState.GetAll())
            if (playerState != null && playerState.SelectedObject == tempBuilding)
                playerState.Select(building);

        Destroy(tempBuilding);
    }

    /// <summary>
    /// Lets the target object know it has been attacked on the client which has authority over it.
    /// </summary>
    /// <param name="attackerId">netId of the attacker</param>
    /// <param name="targetId">netId of the target</param>
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

    /// <summary>
    /// Removes the object from the pathfinding graph.
    /// </summary>
    /// <param name="center">center of the object's bounds</param>
    /// <param name="size">size of the object's bounds</param>
    [ClientRpc]
    public void RpcDestroyObject(Vector3 center, Vector3 size)
    {
        var guo = new GraphUpdateObject(new Bounds(center, size))
        {
            modifyWalkability = true,
            setWalkability = true,
            updatePhysics = true
        };
        AstarPath.active?.UpdateGraphs(guo);
    }
}