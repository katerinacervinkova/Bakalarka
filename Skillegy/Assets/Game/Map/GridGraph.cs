using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GridGraph : NetworkBehaviour {

    Selectable[][] graph;
    int minx;
    int minz;

    private void Awake()
    {
        CreateGraph();
    }

    private void Update()
    {
        for (int i = 0; i < graph.Length; i++)
        {
            for (int j = 0; j < graph[i].Length; j++)
            {
                var place = GraphToWorldCoordinates(new Vector2Int(i, j));
                if (IsOccupied(place))
                    Debug.DrawRay(GraphToWorldCoordinates(new Vector2Int(i, j)), Vector3.up, Color.red);
                //else
                  //  Debug.DrawRay(GraphToWorldCoordinates(new Vector2Int(i, j)), Vector3.up);
            }
        }
    }
    public Vector3? ClosestUnoccupiedDestination(Vector3 location)
    {
        Vector2Int loc = WorldToGraphCoordinates(location);

        if (!IsOccupied(loc))
            return GraphToWorldCoordinates(loc);

        var enumerator = GetEnumerator(loc);
        enumerator.MoveNext();
        for (var vector = enumerator.Current; enumerator.MoveNext(); vector = enumerator.Current)
            if (!IsOccupied(vector))
                return GraphToWorldCoordinates(vector);
        return null;
    }

    internal bool IsOccupied(TemporaryBuilding buildingToBuild, Vector2Int posDelta)
    {
        var position = WorldToGraphCoordinates(buildingToBuild.transform.position);

        for (int i = Math.Max(0, position.x - posDelta.x); i <= Math.Min(graph.Length - 1, position.x + posDelta.x); i++)
            for (int j = Math.Max(0, position.y - posDelta.y); j <= Math.Min(graph[i].Length - 1, position.y + posDelta.y); j++)
                if (graph[i][j] != null)
                    return true;
        return false;
    }

    public bool IsOccupied(Vector3 location) => Get(location) != null;

    public bool IsOccupied(Vector2Int location) => Get(location) != null;

    public Vector3 ClosestDestination(Vector3 location) => GraphToWorldCoordinates(WorldToGraphCoordinates(location));

    public void SetPoint(Selectable selectable, Vector3 selectablePos)
    {
        var position = WorldToGraphCoordinates(selectablePos);
        graph[position.x][position.y] = selectable;
    }

    public void SetRect(Selectable selectable, Vector3 selectablePos, Vector2Int posDelta)
    {
        var position = WorldToGraphCoordinates(selectablePos);

        var enumerator = GetEnumerator(position, Math.Max(posDelta.x, posDelta.y));
        enumerator.MoveNext();

        for (var v = enumerator.Current; enumerator.MoveNext(); v = enumerator.Current)
            if (Math.Abs(position.x - v.x) <= posDelta.x && Math.Abs(position.y - v.y) <= posDelta.y)
                graph[v.x][v.y] = selectable;
    }

    public void SetCircle(Selectable selectable, Vector3 selectablePos, int r)
    {
        if (selectablePos.x == -34 && selectablePos.z == 31)
            Debug.Log("jdkfj");
        var position = WorldToGraphCoordinates(selectablePos);

        var enumerator = GetEnumerator(position, r);
        enumerator.MoveNext();

        for (var v = enumerator.Current; enumerator.MoveNext(); v = enumerator.Current)
            if (Vector2.Distance(position, v) <= r)
                graph[v.x][v.y] = selectable;
    }

    public T GetNearbyResource<T>(Vector3 position, int maxSize, int minSize) where T : Resource
    {
        Vector2Int pos = WorldToGraphCoordinates(position);

        var enumerator = GetEnumerator(pos, maxSize, minSize);
        enumerator.MoveNext();

        for (var v = enumerator.Current; enumerator.MoveNext(); v = enumerator.Current)
            if (Get(v) is T)
            {
                Debug.DrawRay(GraphToWorldCoordinates(v), Vector3.up * 5, Color.yellow, 10);
                return (T)Get(v);
            }
        return null;
    }

    private Selectable Get(Vector3 location) => Get(WorldToGraphCoordinates(location));
    private Selectable Get(Vector2Int location) => graph[location.x][location.y];

    private void CreateGraph()
    {
        Bounds bounds = gameObject.GetComponent<Collider>().bounds;
        minx = (int)Math.Ceiling(bounds.min.x);
        minz = (int)Math.Ceiling(bounds.min.z);
        int maxx = (int)Math.Ceiling(bounds.max.x);
        if (maxx == bounds.max.x)
            maxx++;
        int maxz = (int)Math.Ceiling(bounds.max.z);
        if (maxz == bounds.max.z)
            maxz++;

        int width = maxx - minx;
        int height = maxz - minz;

        graph = new Selectable[height][];
        for (int i = 0; i < height; i++)
            graph[i] = new Selectable[width];
    }

    private Vector3 GraphToWorldCoordinates(Vector2Int pos) => new Vector3(pos.x + minx, 0, pos.y + minz);
    private Vector2Int WorldToGraphCoordinates(Vector3 pos) => new Vector2Int((int)Math.Round(pos.x) - minx, (int)Math.Round(pos.z) - minz);

    private IEnumerator<Vector2Int> GetEnumerator(Vector2Int loc, int maxSize = int.MaxValue, int minSize = 0)
    {
        if (minSize == 0)
            yield return loc;
        for (int i = minSize + 1; i <= maxSize; i++)
        {
            for (int j = Math.Min(graph[0].Length, loc.x + i); j > Math.Max(0, loc.x - i); j--)
                yield return new Vector2Int(j, loc.y - i);
            for (int j = Math.Max(0, loc.y - i); j < Math.Min(graph.Length, loc.y + i); j++)
                yield return new Vector2Int(loc.x - i, j);
            for (int j = Math.Max(0, loc.x - i); j < Math.Min(graph[0].Length, loc.x + i); j++)
                yield return new Vector2Int(j, loc.y + i);
            for (int j = Math.Min(graph.Length, loc.y + i); j > Math.Max(0, loc.y - i); j--)
                yield return new Vector2Int(loc.x + i, j);
        }
        yield return loc;
    }
}
