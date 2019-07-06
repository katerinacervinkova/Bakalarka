using System;
using UnityEngine;

public class Sawmill : Building
{
    public override string Name => "Sawmill";
    public override string UnitText(Unit unit) => $"Gathering: {(int)unit.Gathering}";

    private readonly float gatheringIncrease = 0.005f;
    private readonly float slowGatheringIncrease = 0.001f;

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        PlayerState.Get(playerId).Wood += unit.Gathering / 2;
        if (unit.Gathering < unit.Intelligence)
            owner.ChangeAttribute(unit, AttEnum.Gathering, unit.Gathering + gatheringIncrease * unit.Intelligence);
        else
            owner.ChangeAttribute(unit, AttEnum.Gathering, unit.Gathering + slowGatheringIncrease * unit.Intelligence);
    }
}
