public abstract class Commandable : Selectable {

    public abstract void SetGoal(Selectable goal);

    protected override void InitPurchases()
    {
        Purchases.Add(new Purchase("Building", "Create Main Building", owner.CreateTemporaryMainBuilding, food: 0, wood: 50, gold: 0));
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
