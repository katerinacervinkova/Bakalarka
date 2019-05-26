using System;
using UnityEngine;

public class Barracks : Building
{
    public override Func<Unit, string> UnitTextFunc => u => $"Swordsmanship: {(int)u.Swordsmanship}";

    public override string Name => "Barracks";

    protected override int MaxPopulationIncrease => 0;

    protected override void ChangeColor()
    {
        transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
        transform.Find("Building/Dog").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        owner.ChangeAttribute(unit, AttEnum.Swordsmanship, unit.Swordsmanship + 0.1f);
    }
}
