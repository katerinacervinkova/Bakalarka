using System;
using UnityEngine;

public class Infirmary : Building
{
    public override string Name => "Infirmary";
    public override string UnitText(Unit unit)
    {
        if (unit == Healer)
            return $"Healing: {(int)unit.Healing}";
        return $"Health: {(int)unit.Health}/{(int)unit.MaxHealth}";
    }

    public Unit Healer = null;

    private readonly float noHealerHealthIncrease = 0.2f;
    private readonly float healthIncrease = 0.05f;
    private readonly float healingIncrease = 0.005f;

    private int maxUnitsHealing = 2;
    public override int UnitCapacity => 20;

    public override string UnitName(Unit unit)
    {
        if (Healer == unit)
            return "Healer";
        return base.UnitName(unit);
    }

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
            owner.ChangeAttribute(unit, SkillEnum.Healing, unit.Healing + healingIncrease * unit.Intelligence);
        else if (Healer == null)
            owner.ChangeHealth(unit, unit.Health + noHealerHealthIncrease);
        else if (unitsInside.Count <= maxUnitsHealing + 1)
            owner.ChangeHealth(unit, unit.Health + healthIncrease * Healer.Healing);
        else
            owner.ChangeHealth(unit, unit.Health + healthIncrease * Healer.Healing * maxUnitsHealing / unitsInside.Count);

    }
}
