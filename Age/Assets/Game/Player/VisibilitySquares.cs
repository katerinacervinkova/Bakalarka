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
        for (int i = -20; i <= 20; i++)
            for (int j = -20; j <= 20; j++)
                squares[new Vector2(i, j)] = Instantiate(squarePrefab, new Vector3(10 * i, 0, 10 * j), Quaternion.identity, map.transform);

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
	}
	
	void Update ()
    {
		foreach(var square in squares.Values)
            if (square.ContainsFriend)
                square.Activate();
        foreach (var square in squares.Values)
            square.UpdateVisibility();
	}

    public Vector2 GetSquare(Vector3 position) => new Vector2((float)Math.Round(position.x / 10), (float)Math.Round(position.z / 10));

    public void AddToSquare(Vector2 square, Selectable selectable)
    {
        if (squares.ContainsKey(square))
            squares[square].Add(selectable);
    }

    public void RemoveFromSquare(Vector2 square, Selectable selectable)
    {
        if (squares.ContainsKey(square))
            squares[square].Remove(selectable);
    }
}
