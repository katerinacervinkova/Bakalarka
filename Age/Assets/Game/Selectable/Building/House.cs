using System;

public class House : Building
{
    public override Func<Unit, string> UnitTextFunc => u => "";

    public override string Name => "House";

    protected override int MaxPopulationIncrease => 10;

    protected override void InitPurchases() { }

    protected override void UpdateUnit(Unit unit) { }
}
