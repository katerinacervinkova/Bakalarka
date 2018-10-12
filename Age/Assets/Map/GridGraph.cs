using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class GridGraph : MonoBehaviour {

    // node[y][x] 
    Selectable[][] graph;
    int minx;
    int minz;


    void Start ()
    {
        CreateGraph();
    }
	
	void Update ()
    {
		
	}

    private void CreateGraph()
    {
        Bounds bounds = gameObject.GetComponent<MeshFilter>().mesh.bounds;
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
        for(int i = 0; i < height; i++)
        {
            graph[i] = new Selectable[width];
            for (int j = 0; j < width; j++)
            {
                Vector3 graphCoordinates = GraphToWorldCoordinates(j, i);
                // NavMeshHit navmeshHit;
                // NavMesh.SamplePosition(graphCoordinates, out navmeshHit, 1, 8);
                // Debug.Log(navmeshHit.position);

                // if (navmeshHit.position.x == j && navmeshHit.position.z == i)
                //     graph[i][j] = null;
                // else
                // {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(graphCoordinates);
                    if (Physics.Raycast(ray, out hit))
                        graph[i][j] = hit.collider.gameObject.GetComponent<Selectable>();
                // }
                Debug.Log(graphCoordinates + " " + ((graph[i][j] == null) ? "null" : graph[i][j].ToString()));
            }
        }
        
    }

    private Vector3 GraphToWorldCoordinates(float x, float z)
    {
        return new Vector3((float)Math.Round(x) + minx, -0.1f, (float)Math.Round(z) + minz);
    }
}
