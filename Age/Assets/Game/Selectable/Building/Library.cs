public class Library : Building {

    public override string Name => "Library";

    protected override void InitPurchases() { }

    protected override void UpdateUnit(Unit unit)
    {
        owner.ChangeAttribute(unit, AttEnum.Intelligence, unit.Intelligence + 0.1f);
    }
}
