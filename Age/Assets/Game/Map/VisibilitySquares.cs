using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisibilitySquares : MonoBehaviour {

    [SerializeField]
    private MapSquare squarePrefab;

    private Dictionary<Vector2, MapSquare> squares = new Dictionary<Vector2, MapSquare>();

    void Start ()
    {
        GameObject map = GameObject.Find("Squares");
        for (int i = -40; i <= 40; i++)
            for (int j = -40; j <= 40; j++)
                squares[new Vector2(i, j)] = Instantiate(squarePrefab, new Vector3(5 * i, 0, 5 * j), Quaternion.identity, map.transform);

        foreach(var key in squares.Keys)
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

    void Update()
    {
        foreach (var square in squares.Values)
            if (square.ContainsFriend)
                square.Activate();
        foreach (var square in squares.Values)
            square.UpdateVisibility();
	}

    public T NearestResource<T>(T resource, Vector2 squareID) where T : Resource
    {
        return (T)squares[squareID].AdjoiningSquares.SelectMany(s => s.Resources).Where(r => r is T && r != resource).OrderBy(r => Vector2.Distance(squareID, r.SquareID)).FirstOrDefault();
    }

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
