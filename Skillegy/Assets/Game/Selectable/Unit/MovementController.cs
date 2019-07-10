using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Networking;

public class MovementController : NetworkBehaviour {

    SyncListVector3 vectorPath = new SyncListVector3();

    private AIUnetPath aiUnetPath;
    private Unit unit;

    public Vector3 destination = Vector3.positiveInfinity;
    public bool IsMoving => !float.IsPositiveInfinity(destination.x);


    private bool causedShowingTarget = false;
    private Vector3 lastPosition;

    private void Awake()
    {
        aiUnetPath = GetComponent<AIUnetPath>();
        unit = GetComponent<Unit>();
    }

    private void Update()
    {
        if (hasAuthority && aiUnetPath.ShouldRecalculatePath)
        {
            // changes the destination a little bit if the unit cannot move in its direction
            if (!HasMoved() && Vector3.Distance(unit.transform.position, destination) > aiUnetPath.endReachedDistance * 2)
            {
                var v = Random.insideUnitCircle * aiUnetPath.endReachedDistance;
                aiUnetPath.destination = destination + new Vector3(v.x, 0, v.y);
                aiUnetPath.endReachedDistance += 0.1f;
            }
            // unit is not blocked anymore, returns variable to its original state
            else if (HasMoved())
                aiUnetPath.endReachedDistance = 0.6f;

            // starts path calculating on the server
            CmdSearchPath(aiUnetPath.destination);

            lastPosition = transform.position;
        }
    }

    public void Go(Vector3 destination)
    {
        // sets new destination
        this.destination = destination;
        ShowTarget();
        aiUnetPath.destination = destination;
        aiUnetPath.endReachedDistance = 0.6f;
    }

    private bool HasMoved()
    {
        // returns true if the unit has moved since last frame
        return Vector3.Distance(lastPosition, transform.position) > 0.1;
    }

    [Command]
    private void CmdSearchPath(Vector3 destination)
    {
        // search path on server
        aiUnetPath.destination = destination;
        aiUnetPath.SearchPath();
    }

    public void OnPathComplete(Path p)
    {
        // synchronizes the path
        if (isServer && !p.error)
        {
            vectorPath.Clear();
            foreach (var vector in p.vectorPath)
                vectorPath.Add(vector);
            RpcOnPathComplete();
        }
    }

    [ClientRpc]
    private void RpcOnPathComplete()
    {
        // reconstructs the path and calls the original OnPathComplete
        List<Vector3> vPath = new List<Vector3>();
        foreach (var vector in vectorPath)
            vPath.Add(vector);
        var path = ABPath.FakePath(vPath);
        aiUnetPath.RpcOnPathComplete(path);
    }

    public void OnTargetReached()
    {
        aiUnetPath.destination = Vector3.positiveInfinity; 
        //informs unit that the job is completed
        unit.OnTargetReached();
        destination = Vector3.positiveInfinity;
    }


    /// <summary>
    /// Called when the unit gets deselected or arrives at its destination. Hides the target.
    /// </summary>
    public void HideTarget()
    {
        if (causedShowingTarget && UIManager.Instance != null)
        {
            UIManager.Instance.HideTarget();
            causedShowingTarget = false;
        }
    }

    /// <summary>
    /// Shows the target for the player to know where the unit is heading.
    /// </summary>
    public void ShowTarget()
    {
        if (unit.owner.IsHuman && PlayerState.Get(unit.playerId).SelectedObject == unit && !float.IsPositiveInfinity(destination.x))
        {
            UIManager.Instance.ShowTarget(destination);
            causedShowingTarget = true;
        }
    }

    private class SyncListVector3 : SyncListStruct<Vector3> { }
}