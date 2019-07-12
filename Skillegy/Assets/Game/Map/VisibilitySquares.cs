using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Base class for visibility squares which is also used directly for AI visibility squares.
/// </summary>
public class VisibilitySquares : MonoBehaviour {

    public int playerId;

    private const int SQUARE_SIZE = 5;

    [SerializeField]
    protected MapSquare squarePrefab;

    protected Dictionary<Vector2, MapSquare> squares = new Dictionary<Vector2, MapSquare>();

    protected virtual void Start()
    {
        CreateSquares();
        AddAllResources();
    }

    /// <summary>
    /// Creates the squares and inits adjacent squares for all of them.
    /// </summary>
    private void CreateSquares()
    {
        int count = GameState.Instance.MapSize / SQUARE_SIZE;
        for (int i = -count; i <= count; i++)
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
    }

    /// <summary>
    /// Adds all resources to the square where they belong.
    /// </summary>
    private void AddAllResources()
    {
        foreach (Resource resource in FindObjectsOfType<Resource>())
        {
            var square = GetSquare(resource.FrontPosition);
            squares[square].Add(resource);
            resource.SquareID = square;
        }
    }

    /// <summary>
    /// Activates and uncoveres the squares which are in sight of some object that belongs to the player. 
    /// Squares stay uncovered for the rest of the game, but the activation changes.
    /// </summary>
    protected virtual void Update()
    {
        foreach (var square in squares.Values)
            square.activated = false;
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

    /// <summary>
    /// Finds the closest resource that an object on given square can see.
    /// </summary>
    /// <typeparam name="T">type of resource</typeparam>
    /// <param name="resource">resource to filter out from the search</param>
    public T ClosestVisibleResource<T>(T resource, Vector2 squareID) where T : Resource => (T)squares[squareID].AdjoiningSquares.SelectMany(s => s.Resources).
        Where(r => r is T && r != resource).OrderBy(r => Vector2.Distance(squareID, r.SquareID)).FirstOrDefault();

    /// <summary>
    /// Finds the closest resource the the given square.
    /// </summary>
    public T ClosestVisibleResource<T>(Vector2 squareId) where T : Resource => (T)VisibleResources().Where(r => r is T).OrderBy(r => Vector2.Distance(squareId, r.SquareID)).FirstOrDefault();

    /// <summary>
    /// Finds the closest enemy unit, building or temporary building that an object on given square can see.
    /// </summary>
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

    /// <summary>
    /// Converts the world positino into square ID.
    /// </summary>
    public Vector2 GetSquare(Vector3 position) => new Vector2((float)Math.Round(position.x / 5), (float)Math.Round(position.z / 5));
    /// <summary>
    /// Converts square ID into world position
    /// </summary>
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
