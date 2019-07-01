using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisibilitySquares : MonoBehaviour {

    public int playerId;

    private const int SQUARE_SIZE = 5;

    [SerializeField]
    protected MapSquare squarePrefab;

    protected Dictionary<Vector2, MapSquare> squares = new Dictionary<Vector2, MapSquare>();

    protected virtual void Start()
    {
        int count = GameState.Instance.MapSize / SQUARE_SIZE;
        for (int i = -count ; i <= count; i++)
            for (int j = -count; j <= count; j++)
            {
                Vector2 squareId = new Vector2(i, j);
                squares[squareId] = Instantiate(squarePrefab, GetPosition(squareId), Quaternion.identity, transform);
                squares[squareId].playerId = playerId;
                squares[squareId].squareId = squareId;
            }

        foreach (var key in squares.Keys)
        {
            var AdjoiningSquares = new List<MapSquare>();
            for (int i = (int)key.x - 3; i < key.x + 4; i++)
            {
                for (int j = (int)key.y - 3; j < key.y + 4; j++)
                {
                    Vector2 neighbour = new Vector2(i, j);
                    if (squares.ContainsKey(neighbour) && Vector2.Distance(neighbour, key) <= 3.5)
                        AdjoiningSquares.Add(squares[neighbour]);
                }
            }
            squares[key].AdjoiningSquares = AdjoiningSquares;
        }

        foreach (Resource resource in FindObjectsOfType<Resource>())
        {
            var square = GetSquare(resource.FrontPosition);
            squares[square].Add(resource);
            resource.SquareID = square;
        }
    }

    protected virtual void Update()
    {
        foreach (var square in squares.Values)
            if (square.ContainsFriend)
                square.AdjoiningSquares.ForEach(s => { s.activated = true; s.uncovered = true; });
    }

    public List<Unit> VisibleEnemyUnits()
    {
        var units = new List<Unit>();
        foreach (var square in squares.Values)
            if (square.wasActive)
                units.AddRange(square.EnemyUnits);
        return units;
    }

    public List<Building> VisibleEnemyBuildings()
    {
        var buildings = new List<Building>();
        foreach (var square in squares.Values)
            if (square.uncovered)
                buildings.AddRange(square.EnemyBuildings);
        return buildings;
    }

    public List<TemporaryBuilding> VisibleEnemyTemporaryBuildings()
    {
        var buildings = new List<TemporaryBuilding>();
        foreach (var square in squares.Values)
            if (square.uncovered)
                buildings.AddRange(square.EnemyTemporaryBuildings);
        return buildings;
    }

    public List<Resource> VisibleResources()
    {
        var resources = new List<Resource>();
        foreach (var square in squares.Values)
            if (square.uncovered)
                resources.AddRange(square.Resources);
        return resources;
    }

    public Vector3 GetClosestFreePosition(Vector3 position)
    {
        Vector2 square = GetSquare(position);
        var sq =  squares[square].AdjoiningSquares.
            Where(s => s.Resources.Count == 0 && s.EnemyBuildings.Count == 0 && s.FriendlyBuildings.Count == 0 && s.EnemyTemporaryBuildings.Count == 0 && s.FriendlyTemporaryBuildings.Count == 0).
            Select(s => GetPosition(s.squareId));
        if (sq.Any())
            return sq.First();
        return Vector3.positiveInfinity;
    }

    public T ClosestVisibleResource<T>(T resource, Vector2 squareID) where T : Resource => (T)squares[squareID].AdjoiningSquares.SelectMany(s => s.Resources).
        Where(r => r is T && r != resource).OrderBy(r => Vector2.Distance(squareID, r.SquareID)).FirstOrDefault();

    public T ClosestGloballyVisibleResource<T>(Vector2 squareId) where T : Resource => (T)VisibleResources().Where(r => r is T).OrderBy(r => Vector2.Distance(squareId, r.SquareID)).FirstOrDefault();

    public Selectable ClosestGloballyVisibleTarget(Vector2 id)
    {
        var unit = VisibleEnemyUnits().OrderBy(u => Vector2.Distance(id, u.SquareID)).FirstOrDefault();
        var tempBuilding = VisibleEnemyTemporaryBuildings().OrderBy(b => Vector2.Distance(id, b.SquareID)).FirstOrDefault();
        var building = VisibleEnemyBuildings().OrderBy(b => Vector2.Distance(id, b.SquareID)).FirstOrDefault();

        var unitDist = float.PositiveInfinity;
        var buildingDist = float.PositiveInfinity;
        var tempBuildingDist = float.PositiveInfinity;

        if (unit != null)
            unitDist = Vector2.Distance(id, unit.SquareID);
        if (building != null)
            buildingDist = Vector2.Distance(id, building.SquareID);
        if (tempBuilding != null)
            tempBuildingDist = Vector2.Distance(id, tempBuilding.SquareID);

        if (unitDist < tempBuildingDist && unitDist < buildingDist)
            return unit;
        if (buildingDist < tempBuildingDist && buildingDist < unitDist)
            return building;
        return tempBuilding;
    }

    public Selectable ClosestVisibleTarget(Vector2 squareId)
    {
        var unit = squares[squareId].AdjoiningSquares.SelectMany(s => s.EnemyUnits).OrderBy(b => Vector2.Distance(squareId, b.SquareID)).FirstOrDefault();
        if (unit != null)
            return unit;
        var building = squares[squareId].AdjoiningSquares.SelectMany(s => s.EnemyTemporaryBuildings).OrderBy(b => Vector2.Distance(squareId, b.SquareID)).FirstOrDefault();
        if (building != null)
            return building;
        return squares[squareId].AdjoiningSquares.SelectMany(s => s.EnemyBuildings).OrderBy(b => Vector2.Distance(squareId, b.SquareID)).FirstOrDefault();
    }

    public Vector2 GetSquare(Vector3 position) => new Vector2((float)Math.Round(position.x / 5), (float)Math.Round(position.z / 5));
    public Vector3 GetPosition(Vector2 squareId) => new Vector3(SQUARE_SIZE * squareId.x, 0, SQUARE_SIZE * squareId.y);

    public void AddToSquare(Vector2 square, Unit unit)
    {
        if (squares.ContainsKey(square))
            squares[square].Add(unit);
    }

    public void RemoveFromSquare(Vector2 square, Unit unit)
    {
        if (squares.ContainsKey(square))
            squares[square].Remove(unit);
    }

    public void AddToSquare(Vector2 square, Building building)
    {
        if (squares.ContainsKey(square))
            squares[square].Add(building);
    }

    public void RemoveFromSquare(Vector2 square, Building building)
    {
        if (squares.ContainsKey(square))
            squares[square].Remove(building);
    }

    public void AddToSquare(Vector2 square, TemporaryBuilding building)
    {
        if (squares.ContainsKey(square))
            squares[square].Add(building);
    }

    public void RemoveFromSquare(Vector2 square, TemporaryBuilding building)
    {
        if (squares.ContainsKey(square))
            squares[square].Remove(building);
    }

    public void RemoveFromSquare(Vector2 square, Resource resource)
    {
        if (squares.ContainsKey(square))
            squares[square].Remove(resource);
    }
}
