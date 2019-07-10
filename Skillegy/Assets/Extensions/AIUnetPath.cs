using Pathfinding;
using UnityEngine;

public class AIUnetPath : AIPath {

    protected Unit unit;

    // making a private member public
    public bool ShouldRecalculatePath => shouldRecalculatePath;

    protected override void Awake()
    {
        base.Awake();
        unit = GetComponent<Unit>();
    }

    public override void OnTargetReached()
    {
        destination = Vector3.positiveInfinity;
        //informs unit that the job is completed
        unit.OnTargetReached();
        unit.movementController.destination = Vector3.positiveInfinity;
    }

    // overrides the implementation with synchronizing the path between clients
    protected override void OnPathComplete(Path newPath)
    {
        unit.movementController.OnPathComplete(newPath);
    }

    // calls the original OnPathCompleteMethod on every client
    public void RpcOnPathComplete(Path newPath)
    {
        base.OnPathComplete(newPath);
    }
}
