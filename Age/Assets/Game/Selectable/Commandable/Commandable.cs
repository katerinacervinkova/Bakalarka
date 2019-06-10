using UnityEngine;

public abstract class Commandable : Selectable {

    protected Vector3 destination = Vector3.positiveInfinity;
    private bool causedShowingTarget = false;
    public bool IsMoving => !float.IsPositiveInfinity(destination.x);


    public abstract void SetGoal(Selectable goal);

    protected void HideTarget()
    {
        if (causedShowingTarget && UIManager.Instance != null)
        {
            UIManager.Instance.HideTarget();
            causedShowingTarget = false;
        }
    }

    protected void ShowTarget()
    {
        if (owner.IsHuman && PlayerState.Get(playerId).SelectedObject == this)
        {
            UIManager.Instance.ShowTarget(destination);
            causedShowingTarget = true;
        }
    }

    public override void RightMouseClickObject(Selectable hitObject)
    {
        SetGoal(hitObject);
    }
    public override Job GetOwnJob(Commandable worker)
    {
        return null;
    }
}
