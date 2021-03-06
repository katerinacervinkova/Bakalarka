﻿using UnityEngine;

public class Bank : Building
{
    public override string Name => "Bank";
    public override string UnitText(Unit unit) => $"Gathering: {(int)unit.Gathering}";

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    /// <summary>
    /// Adds some amount of gold to the player and increases unit's Gathering level.
    /// </summary>
    protected override void UpdateUnit(Unit unit)
    {
        PlayerState.Get(playerId).Gold += unit.Gathering / 2;
        owner.ChangeAttribute(unit, SkillEnum.Gathering, unit.Gathering + 0.05f);
    }
}
