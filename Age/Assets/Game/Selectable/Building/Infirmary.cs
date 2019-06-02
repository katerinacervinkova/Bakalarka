using System;
using UnityEngine;

public class Infirmary : Building
{
    public override Func<Unit, string> UnitTextFunc => u => $"Health: {(int)u.Health}/{(int)u.MaxHealth}, Healing: {(int)u.Healing}";

    public override string Name => "Infirmary";

    protected override int MaxPopulationIncrease => 0;

    protected override void ChangeColor()
    {
        transform.Find("Building/Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        owner.ChangeHealth(unit, unit.Health + 0.1f);
    }
}
