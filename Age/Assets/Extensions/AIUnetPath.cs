using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnetPath : AIPath {

    protected Unit unit;

    public bool ShouldRecalculatePath => shouldRecalculatePath;

    protected override void Awake()
    {
        base.Awake();
        unit = GetComponent<Unit>();
    }

    public override void OnTargetReached()
    {
        destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        unit.OnTargetReached();
    }

    protected override void OnPathComplete(Path newPath)
    {
        unit.movementController.OnPathComplete(newPath);
    }

    public void RpcOnPathComplete(Path newPath)
    {
        base.OnPathComplete(newPath);
    }
}
