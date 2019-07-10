using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Networking;

public class MovementController : NetworkBehaviour {

    SyncListVector3 vectorPath = new SyncListVector3();

    private AIUnetPath aiUnetPath;
    private Unit unit;

    // variables related to movement
    public Vector3 destination = Vector3.positiveInfinity;

    private bool causedShowingTarget = false;
    public bool IsMoving => !float.IsPositiveInfinity(destination.x);
    private Vector3 lastPosition;

    public void OnTargetReached()
    {
        aiUnetPath.destination = Vector3.positiveInfinity;
        unit.OnTargetReached();
        destination = Vector3.positiveInfinity;
    }

    private void Awake()
    {
        aiUnetPath = GetComponent<AIUnetPath>();
        unit = GetComponent<Unit>();
    }

    private void Update()
    {
        if (hasAuthority && aiUnetPath.ShouldRecalculatePath)
        {
            if (!HasMoved() && Vector3.Distance(unit.transform.position, destination) > aiUnetPath.endReachedDistance * 2)
            {
                var v = Random.insideUnitCircle * aiUnetPath.endReachedDistance;
                aiUnetPath.destination = destination + new Vector3(v.x, 0, v.y);
                aiUnetPath.endReachedDistance += 0.1f;
            }
            else if (HasMoved())
                aiUnetPath.endReachedDistance = 0.6f;
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

    public void Go(Vector3 destination)
    {
        this.destination = destination;
        ShowTarget();
        aiUnetPath.destination = destination;
        aiUnetPath.endReachedDistance = 0.6f;
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