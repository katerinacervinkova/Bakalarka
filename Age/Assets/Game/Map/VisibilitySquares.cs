using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisibilitySquares : MonoBehaviour {

    public int playerId;

    [SerializeField]
    protected MapSquare squarePrefab;

    protected Dictionary<Vector2, MapSquare> squares = new Dictionary<Vector2, MapSquare>();

    void Start()
    {
        for (int i = -40; i <= 40; i++)
            for (int j = -40; j <= 40; j++)
            {
                squares[new Vector2(i, j)] = Instantiate(squarePrefab, new Vector3(5 * i, 0, 5 * j), Quaternion.identity, transform);
                squares[new Vector2(i, j)].playerId = playerId;
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
                square.Activate();
    }

    public List<Unit> VisibleEnemyUnits()
    {
        var units = new List<Unit>();
        foreach (var square in squares.Values)
            if (square.activated)
                units.AddRange(square.EnemyUnits);
        return units;
    }

    public List<Building> VisibleEnemyBuildings()
    {
        var buildings = new List<Building>();
        foreach (var square in squares.Values)
            if (square.activated)
                buildings.AddRange(square.EnemyBuildings);
        return buildings;
    }

    public List<TemporaryBuilding> VisibleEnemyTemporaryBuildings()
    {
        var buildings = new List<TemporaryBuilding>();
        foreach (var square in squares.Values)
            if (square.activated)
                buildings.AddRange(square.EnemyTemporaryBuildings);
        return buildings;
    }

    public List<Resource> VisibleResources()
    {
        var resources = new List<Resource>();
        foreach (var square in squares.Values)
            if (square.activated)
                resources.AddRange(square.Resources);
        return resources;
    }

    public T ClosestResource<T>(T resource, Vector2 squareID) where T : Resource => (T)squares[squareID].AdjoiningSquares.SelectMany(s => s.Resources).
        Where(r => r is T && r != resource).OrderBy(r => Vector2.Distance(squareID, r.SquareID)).FirstOrDefault();

    public T ClosestVisibleResource<T>(Vector2 squareId) where T : Resource => (T)squares[squareId].AdjoiningSquares.Where(s => s.activated).SelectMany(s => s.Resources).
        Where(r => r is T).OrderBy(r => Vector2.Distance(squareId, r.SquareID)).FirstOrDefault();

    public Vector2 GetSquare(Vector3 position) => new Vector2((float)Math.Round(position.x / 5), (float)Math.Round(position.z / 5));

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
