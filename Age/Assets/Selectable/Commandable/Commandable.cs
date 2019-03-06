using UnityEngine;

public abstract class Commandable : Selectable {

    public abstract void SetGoal(Selectable goal);
    public override void RightMouseClickObject(Selectable hitObject)
    {
        SetGoal(hitObject);
    }
    protected override Job CreateOwnJob(Commandable worker)
    {
        return null;
    }
    protected override Job CreateEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }
}
