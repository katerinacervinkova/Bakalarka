using System;
using UnityEngine;

public class MainBuilding : Building {

    public override string Name => "Main building";

    public override Func<Unit, string> UnitTextFunc => u => $"Health: {u.Health}";

    protected override int MaxPopulationIncrease => 5;

    protected override void InitPurchases()
    {
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.Unit));
    }

    protected override void UpdateUnit(Unit unit) { }
}
