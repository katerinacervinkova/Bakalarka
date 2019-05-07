using System;
using UnityEngine;

public class MainBuilding : Building {

    public override string Name => "Main building";

    public override Func<Unit, string> UnitTextFunc => u => $"Health: {u.Health}";

    protected override void InitPurchases()
    {
        Purchases.Add(new LoadingPurchase(1, this, "Unit", "Create a unit", () => owner.CreateUnit(this), food: 20, wood: 0, gold: 0));
    }

    protected override void UpdateUnit(Unit unit) { }
}
