using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Building
{
    public override Func<Unit, string> UnitTextFunc => u => "";

    public override string Name => "House";

    protected override void InitPurchases() { }

    protected override void UpdateUnit(Unit unit) { }
}
