using System;
using UnityEngine;

public class House : Building
{
    public override string Name => "House";
    public override string UnitText(Unit unit) => "";

    public override int UnitCapacity => 1;

    protected override int MaxPopulationIncrease => 10;

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit) { }
}
