using UnityEngine;

public class Library : Building {

    public override string Name => "Library";

    protected override void InitPurchases() { }

    protected override void UpdateUnit(Unit unit)
    {
        owner.ChangeAttribute(unit, AttEnum.Intelligence, unit.Intelligence + 0.1f);
    }

    protected override void ChangeColour()
    {
        transform.Find("Library/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }
}
