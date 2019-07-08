using System;
using UnityEngine;

public class MainBuilding : Building {

    public override string Name => "Main building";
    public override string UnitText(Unit unit) => $"Health: {unit.Health}";
    public override int UnitCapacity => 100;

    protected override int MaxPopulationIncrease => 5;


    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Main Roof").GetComponent<MeshRenderer>().material.color = owner.color;
        visibleObject.transform.Find("Building/Roof 1").GetComponent<MeshRenderer>().material.color = owner.color;
        visibleObject.transform.Find("Building/Roof 2").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void InitPurchases()
    {
        AddPurchase(PurchasesEnum.Unit);
        AddPurchase(PurchasesEnum.StoneAge);
        AddPurchase(PurchasesEnum.IronAge);
        AddPurchase(PurchasesEnum.DiamondAge);
    }

    protected override void UpdateUnit(Unit unit) { }
}
