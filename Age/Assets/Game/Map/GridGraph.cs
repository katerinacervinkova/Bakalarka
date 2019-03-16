using System;
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
                var place = GraphToWorldCoordinates(i, j);
                if (IsOccupied(place))
                    Debug.DrawRay(GraphToWorldCoordinates(i, j), Vector3.up, Color.red);
                else
                    Debug.DrawRay(GraphToWorldCoordinates(i, j), Vector3.up);
            }
        }
    }
    public Vector3 ClosestUnoccupiedDestination(Vector3 location)
    {
        Vector3Int loc = WorldToGraphCoordinates(location);
        if (!IsOccupied(loc, false))
            return GraphToWorldCoordinates(loc);
        Vector3 newLocation = Vector3Int.zero;
        for (int i = 0;; i++)
        {
            for (int j = loc.x + i; j > loc.x - i; j--)
                if (getLocation(ref newLocation, j, loc.z - i))
                    return newLocation;
            for (int j = loc.z - i; j < loc.z + i; j++)
                if (getLocation(ref newLocation, loc.x - i, j))
                    return newLocation;
            for (int j = loc.x - i; j < loc.x + i; j++)
                if (getLocation(ref newLocation, j, loc.z + i))
                    return newLocation;
            for (int j = loc.z + i; j > loc.z - i; j--)
                if (getLocation(ref newLocation, loc.x + i, j))
                    return newLocation;
        }
    }

    internal bool IsOccupied(TemporaryBuilding buildingToBuild)
    {
        Bounds bounds = buildingToBuild.GetComponent<Collider>().bounds;
        Vector3Int min = WorldToGraphCoordinates(bounds.min);
        Vector3Int max = WorldToGraphCoordinates(bounds.max);
        for (int i = Math.Max(0, min.x); i <= Math.Min(max.x, graph.Length - 1); i++)
            for (int j = Math.Max(0, min.z); j <= Math.Min(max.z, graph[i].Length - 1); j++)
                if (graph[i][j] != null)
                    return true;
        return false;

    }

    private bool getLocation(ref Vector3 newLocation, int x, int z)
    {
        newLocation = new Vector3(x, 0, z);
        if (!IsOccupied(newLocation, false))
        {
            newLocation = GraphToWorldCoordinates(newLocation);
            return true;
        }
        return false;
    }

    public Vector3 ClosestDestination(Vector3 location)
    {
        return GraphToWorldCoordinates(WorldToGraphCoordinates(location));
    }

    public void Remove(Selectable selectable)
    {
        if (selectable == null)
            return;
        for (int i = 0; i < graph.Length; i++)
            for (int j = 0; j < graph[i].Length; j++)
                if (graph[i][j] == selectable)
                    graph[i][j] = null;
    }

    public void Add(Selectable selectable)
    {
        Bounds bounds = selectable.GetComponent<Collider>().bounds;
        Vector3Int min = WorldToGraphCoordinates(bounds.min);
        Vector3Int max = WorldToGraphCoordinates(bounds.max);

        for (int i = min.x; i <= max.x; i++)
            for (int j = min.z; j <= max.z; j++)
                graph[i][j] = selectable;
    }


    public bool IsOccupied(Vector3 location, bool worldCoordinates = true)
    {
        return Get(location, worldCoordinates) != null;
    }

    private Selectable Get(Vector3 location, bool worldCoordinates = true)
    {
        if (worldCoordinates)
            location = WorldToGraphCoordinates(location);
        return graph[(int)location.x][(int)location.z];
    }

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

    private Vector3 GraphToWorldCoordinates(int x, int z)
    {
        return new Vector3(x + minx, 0, z + minz);
    }

    private Vector3 GraphToWorldCoordinates(Vector3 position)
    {
        return GraphToWorldCoordinates((int)position.x, (int)position.z);
    }

    private Vector3Int WorldToGraphCoordinates(float x, float z)
    {
        return new Vector3Int((int)Math.Round(x) - minx, 0, (int)Math.Round(z) - minz);
    }

    private Vector3Int WorldToGraphCoordinates(Vector3 position)
    {
        return WorldToGraphCoordinates(position.x, position.z);
    }
}
