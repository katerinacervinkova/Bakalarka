using UnityEngine;

public class MainBuilding : Building {

    public override string Name => "Main building";

    protected override void InitPurchases()
    {
        Purchases.Add(new LoadingPurchase(1, this, "Unit", "Create a unit", () => owner.CreateUnit(this), food: 20, wood: 0, gold: 0));
    }

    protected override void UpdateUnit(Unit unit) { }

    protected override void ChangeColour()
    {
        transform.Find("Floor1").GetComponent<MeshRenderer>().material.color = owner.color;
        transform.Find("Floor2").GetComponent<MeshRenderer>().material.color = owner.color;
    }
}
