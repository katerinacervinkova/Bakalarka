using System;
using UnityEngine;

public class Sawmill : Building
{
    public override Func<Unit, string> UnitTextFunc => u => $"Gathering: {(int)u.Gathering}";

    public override string Name => "Sawmill";

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        PlayerState.Get(playerId).Wood += unit.Gathering / 2;
        owner.ChangeAttribute(unit, AttEnum.Gathering, unit.Gathering + 0.05f);
    }
}
