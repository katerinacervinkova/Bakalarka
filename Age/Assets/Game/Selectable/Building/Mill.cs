using System;
using UnityEngine;

public class Mill : Building
{
    public override Func<Unit, string> UnitTextFunc => u => $"Gathering: {(int)u.Gathering}";

    public override string Name => "Mill";

    protected override int MaxPopulationIncrease => 0;

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Small roof").GetComponent<MeshRenderer>().material.color = owner.color;
        visibleObject.transform.Find("Building/Big roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        PlayerState.Get(playerId).Food += unit.Gathering / 2;
        owner.ChangeAttribute(unit, AttEnum.Gathering, unit.Gathering + 0.05f);
    }
}
