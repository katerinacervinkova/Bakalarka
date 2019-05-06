using UnityEngine;

public abstract class Commandable : Selectable {

    protected Vector3 destination = Vector3.positiveInfinity;
    private bool causedShowingTarget = false;
    public bool IsMoving => !float.IsPositiveInfinity(destination.x);


    public abstract void SetGoal(Selectable goal);

    protected void HideTarget()
    {
        if (causedShowingTarget)
        {
            UIManager.Instance.HideTarget();
            causedShowingTarget = false;
        }
    }

    protected void ShowTarget()
    {
        if (PlayerState.Instance.SelectedObject == this)
        {
            UIManager.Instance.ShowTarget(destination);
            causedShowingTarget = true;
        }
    }

    protected override void InitPurchases()
    {
        Purchases.Add(new Purchase("Main building", "Create Main Building", () => owner.CreateTempBuilding(BuildingEnum.MainBuilding), food: 0, wood: 50, gold: 0));
        Purchases.Add(new Purchase("Library", "Create Library, which increases units' intelligence", () => owner.CreateTempBuilding(BuildingEnum.Library), food: 0, wood: 20, gold: 10));
    }

    public override void RightMouseClickObject(Selectable hitObject)
    {
        SetGoal(hitObject);
    }
    public override Job GetOwnJob(Commandable worker)
    {
        return null;
    }
    public override Job GetEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }
}
