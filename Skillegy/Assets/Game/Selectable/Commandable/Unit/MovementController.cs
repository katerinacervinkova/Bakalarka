using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Networking;

public class MovementController : NetworkBehaviour {

    SyncListVector3 vectorPath = new SyncListVector3();

    AIUnetPath aiUnetPath;

    Vector3 lastPosition;

    private void Awake()
    {
        aiUnetPath = GetComponent<AIUnetPath>();
    }

    private void Update()
    {
        if (hasAuthority && aiUnetPath.ShouldRecalculatePath)
        {
            if (!HasMoved())
            {
                var v = Random.insideUnitCircle * aiUnetPath.endReachedDistance;
                aiUnetPath.destination += new Vector3(v.x, 0, v.y);
                aiUnetPath.endReachedDistance += 0.1f;
            }
            CmdSearchPath(aiUnetPath.destination);
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
        aiUnetPath.destination = destination;
        aiUnetPath.SearchPath();
    }

    [ClientRpc]
    private void RpcOnPathComplete()
    {
        List<Vector3> vPath = new List<Vector3>();
        foreach (var vector in vectorPath)
            vPath.Add(vector);
        var path = ABPath.FakePath(vPath);
        aiUnetPath.RpcOnPathComplete(path);
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