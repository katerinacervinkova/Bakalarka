using System;
using UnityEngine;

public class MainBuilding : Building {

    public override string Name => "Main building";

    public override Func<Unit, string> UnitTextFunc => u => $"Health: {u.Health}";

    protected override int MaxPopulationIncrease => 5;

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Main Roof").GetComponent<MeshRenderer>().material.color = owner.color;
        visibleObject.transform.Find("Building/Roof 1").GetComponent<MeshRenderer>().material.color = owner.color;
        visibleObject.transform.Find("Building/Roof 2").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void InitPurchases()
    {
        Purchases.Add(PlayerState.Get(playerId).playerPurchases.Get(PurchasesEnum.Unit));
    }

    protected override void UpdateUnit(Unit unit) { }
}
