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

    /// <summary>
    /// Adds some amount of food to the player and increases unit's Gathering level.
    /// </summary>
    protected override void UpdateUnit(Unit unit)
    {
        PlayerState.Get(playerId).Wood += unit.Gathering / 2;
        if (unit.Gathering < unit.Intelligence)
            owner.ChangeAttribute(unit, SkillEnum.Gathering, unit.Gathering + gatheringIncrease * unit.Intelligence);
        else
            owner.ChangeAttribute(unit, SkillEnum.Gathering, unit.Gathering + slowGatheringIncrease * unit.Intelligence);
    }
}
