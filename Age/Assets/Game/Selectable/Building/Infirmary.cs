using System;
using UnityEngine;

public class Infirmary : Building
{
    private readonly float noHealerHealthIncrease = 0.2f;
    private readonly float healthIncrease = 0.05f;

    private int maxUnitsHealing = 2;

    protected override int UnitCapacity => 20;

    private readonly float healingIncrease = 0.005f;
    public Unit Healer = null;

    public override Func<Unit, string> UnitTextFunc => u => {
        if (u == Healer)
            return $"Healing: {(int)u.Healing}";
        return $"Health: {(int)u.Health}/{(int)u.MaxHealth}";
        };
    public override string UnitName(Unit unit)
    {
        if (Healer == unit)
            return "Healer";
        return base.UnitName(unit);
    }

    public override string Name => "Infirmary";

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    public override void ShowUnitsWindow()
    {
        UIManager.Instance.ShowBuildingWindow(this, unitsInside, u => { if (Healer == u) Healer = null; else Healer = u; });
    }
    protected override void UpdateUnit(Unit unit)
    {
        if (unit == Healer)
            owner.ChangeAttribute(unit, AttEnum.Healing, unit.Healing + healingIncrease * unit.Intelligence);
        else if (Healer == null)
            owner.ChangeHealth(unit, unit.Health + noHealerHealthIncrease);
        else if (unitsInside.Count <= maxUnitsHealing + 1)
            owner.ChangeHealth(unit, unit.Health + healthIncrease * Healer.Healing);
        else
            owner.ChangeHealth(unit, unit.Health + healthIncrease * Healer.Healing * maxUnitsHealing / unitsInside.Count);

    }
}
