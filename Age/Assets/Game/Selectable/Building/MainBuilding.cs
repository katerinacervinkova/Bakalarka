using System;
using UnityEngine;

public class MainBuilding : Building {

    public override string Name => "Main building";

    public override Func<Unit, string> UnitTextFunc => u => $"Health: {u.Health}";

    protected override int MaxPopulationIncrease => 5;

    protected override void ChangeColor()
    {
        transform.Find("Building/Building/Main Roof").GetComponent<MeshRenderer>().material.color = owner.color;
        transform.Find("Building/Building/Roof 1").GetComponent<MeshRenderer>().material.color = owner.color;
        transform.Find("Building/Building/Roof 2").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void InitPurchases()
    {
        Purchases.Add(PlayerState.Instance.playerPurchases.Get(PurchasesEnum.Unit));
    }

    protected override void UpdateUnit(Unit unit) { }
}
