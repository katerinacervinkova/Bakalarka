using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Networking;

public class MovementController : NetworkBehaviour {

    SyncListVector3 vectorPath = new SyncListVector3();

    AIPath aIPath;

    public Vector3 destination;

    Vector3 lastPosition;

    private void Awake()
    {
        aIPath = GetComponent<AIPath>();
    }

    private void Update()
    {
        if (hasAuthority && aIPath.shouldRecalculatePath)
        {
            if (!HasMoved())
            {
                var v = Random.insideUnitCircle * aIPath.endReachedDistance;
                aIPath.destination += new Vector3(v.x, 0, v.y);
                aIPath.endReachedDistance += 0.1f;
            }
            CmdSearchPath(aIPath.destination);
            lastPosition = transform.position;
        }
    }

    private bool HasMoved()
    {
        return Vector3.Distance(lastPosition, transform.position) > 0.1;
    }

    [Command]
    private void CmdSearchPath(Vector3 destination)
    {
        aIPath.destination = destination;
        aIPath.SearchPath();
    }

    [ClientRpc]
    private void RpcOnPathComplete()
    {
        List<Vector3> vPath = new List<Vector3>();
        foreach (var vector in vectorPath)
            vPath.Add(vector);
        var path = ABPath.FakePath(vPath);
        aIPath.RpcOnPathComplete(path);
    }

    public void OnPathComplete(Path p)
    {
        if (isServer && !p.error)
        {
            vectorPath.Clear();
            foreach (var vector in p.vectorPath)
                vectorPath.Add(vector);
            RpcOnPathComplete();
        }
    }

    private class SyncListVector3 : SyncListStruct<Vector3> { }
}