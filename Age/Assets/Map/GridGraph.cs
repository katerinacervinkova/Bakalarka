using System;
using UnityEngine;
using UnityEngine.AI;

public class GridGraph : MonoBehaviour {

    Selectable[][] graph;
    int minx;
    int minz;

    private void Awake()
    {
        CreateGraph();
    }

    void Start()
    {
        
    }

    void Update()
    {

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

    public void AddSelectable(Selectable selectable)
    {
        UpgradeGraph(selectable, selectable);
    }
    public void RemoveSelectable(Selectable selectable)
    {
        UpgradeGraph(selectable);
    }

    public void SetDestination(Vector3 destination, NavMeshAgent agent)
    {
        Vector3Int graphCoordinates = WorldToGraphCoordinates(destination);
        if (graph[graphCoordinates.z][graphCoordinates.x] == null)
        {
            destination = GraphToWorldCoordinates(graphCoordinates);
            agent.SetDestination(destination);
        }
    }

    private Vector3 getDestinationCircle(Vector3 startDestination)
    {
        Vector3 destination;
        Vector3Int graphCoordinates = WorldToGraphCoordinates(startDestination);
        int r = 0;
        while(true)
        {
            int xmin = Math.Min(graphCoordinates.x - r, 0);
            int xmax = Math.Max(graphCoordinates.x + r, graph.Length);
            int zmin = Math.Min(graphCoordinates.z - r, 0);
            int zmax = Math.Max(graphCoordinates.z + r, graph[0].Length);

            for (int i = zmin; i < zmax; i++)
            {
                if (getDestination(new Vector3Int(xmin, graphCoordinates.y, i), out destination))
                    return destination;
                if (getDestination(new Vector3Int(xmax, graphCoordinates.y, i), out destination))
                    return destination;
            }
            for (int i = xmin + 1; i < xmax - 1; i++)
            {
                if (getDestination(new Vector3Int(i, graphCoordinates.y, zmin), out destination))
                    return destination;
                if (getDestination(new Vector3Int(i, graphCoordinates.y, zmax), out destination))
                    return destination;
            }
            r++;
        }

    }

    private bool getDestination(Vector3Int graphDestination, out Vector3 destination)
    {
        if (graph[graphDestination.z][graphDestination.x] == null)
        {
            destination = GraphToWorldCoordinates(graphDestination);
            return true;
        }
        destination = Vector3.zero;
        return false;
    }

    private void UpgradeGraph(Selectable selectable, Selectable field = null)
    {
        Bounds bounds = selectable.GetComponent<Collider>().bounds;
        for (int i = (int)Math.Round(bounds.min.z); i <= (int)Math.Round(bounds.max.z); i++)
            for (int j = (int)Math.Round(bounds.min.x); j < (int)Math.Round(bounds.max.x); j++)
            {
                Vector3Int graphCoordinates = WorldToGraphCoordinates(j, i);
                graph[graphCoordinates.z][graphCoordinates.x] = field;
            }
    }

    private Vector3 GraphToWorldCoordinates(int x, int z)
    {
        return new Vector3(x + minx, 0, z + minz);
    }

    public Vector3[] GroupDestinations(Vector3 destination, int count)
    {
        Vector3[] destinations = new Vector3[count];
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
