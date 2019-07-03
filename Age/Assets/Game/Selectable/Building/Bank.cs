using System;
using UnityEngine;

public class Bank : Building
{
    public override Func<Unit, string> UnitTextFunc => u => $"Gathering: {(int)u.Gathering}";

    public override string Name => "Bank";

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        PlayerState.Get(playerId).Gold += unit.Gathering / 2;
        owner.ChangeAttribute(unit, AttEnum.Gathering, unit.Gathering + 0.05f);
    }
}
