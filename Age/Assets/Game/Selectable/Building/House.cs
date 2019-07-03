using System;
using UnityEngine;

public class House : Building
{
    public override Func<Unit, string> UnitTextFunc => u => "";

    public override string Name => "House";

    protected override int UnitCapacity => 1;

    protected override int MaxPopulationIncrease => 10;

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit) { }
}
