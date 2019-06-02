using System;
using UnityEngine;

public class Library : Building {

    public override string Name => "Library";

    public override Func<Unit, string> UnitTextFunc => u => $"Intelligence: {(int)u.Intelligence}";

    protected override int MaxPopulationIncrease => 0;

    protected override void ChangeColor()
    {
        transform.Find("Building/Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        owner.ChangeAttribute(unit, AttEnum.Intelligence, unit.Intelligence + 0.1f);
    }
}
