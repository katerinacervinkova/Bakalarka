using System;

public class Barracks : Building
{
    public override Func<Unit, string> UnitTextFunc => u => $"Swordsmanship: {(int)u.Swordsmanship}";

    public override string Name => "Barracks";

    protected override int MaxPopulationIncrease => 0;

    protected override void InitPurchases() { }

    protected override void UpdateUnit(Unit unit)
    {
        owner.ChangeAttribute(unit, AttEnum.Swordsmanship, unit.Swordsmanship + 0.1f);
    }
}
