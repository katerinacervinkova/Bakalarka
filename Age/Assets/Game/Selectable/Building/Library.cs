using System;

public class Library : Building {

    public override string Name => "Library";

    public override Func<Unit, string> UnitTextFunc => u => $"Intelligence: {(int)u.Intelligence}";

    protected override int MaxPopulationIncrease => 0;

    protected override void InitPurchases() { }

    protected override void UpdateUnit(Unit unit)
    {
        owner.ChangeAttribute(unit, AttEnum.Intelligence, unit.Intelligence + 0.1f);
    }
}
