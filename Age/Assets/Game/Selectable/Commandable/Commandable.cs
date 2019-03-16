using UnityEngine;
using UnityEngine.Networking;

public abstract class Commandable : Selectable {

    public abstract void SetGoal(Selectable goal);

    protected override void InitTransactions()
    {
        if (!hasAuthority)
            return;
        Transactions.Add(new Transaction("Create Main Building", 50, owner.CreateTemporaryMainBuilding));
    }

    public override void RightMouseClickObject(Selectable hitObject)
    {
        SetGoal(hitObject);
    }
    protected override Job GetOwnJob(Commandable worker)
    {
        return null;
    }
    protected override Job GetEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }
}
