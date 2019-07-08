using System;
using UnityEngine;

public class Mill : Building
{
    public override string Name => "Mill";
    public override string UnitText(Unit unit) => $"Gathering: {(int)unit.Gathering}";

    public bool Flour = false;
    public bool Bread = false;

    public float Speed = 0.2f;

    private readonly float gatheringIncrease = 0.005f;
    private readonly float slowGatheringIncrease = 0.001f;

    protected override void InitPurchases()
    {
        AddPurchase(PurchasesEnum.Flour);
        AddPurchase(PurchasesEnum.Bread);
    }

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Small roof").GetComponent<MeshRenderer>().material.color = owner.color;
        visibleObject.transform.Find("Building/Big roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void UpdateUnit(Unit unit)
    {
        PlayerState.Get(playerId).Food += unit.Gathering * Speed;
        if (unit.Gathering < unit.Intelligence)
            owner.ChangeAttribute(unit, AttEnum.Gathering, unit.Gathering + gatheringIncrease * unit.Intelligence);
        else
            owner.ChangeAttribute(unit, AttEnum.Gathering, unit.Gathering + slowGatheringIncrease * unit.Intelligence);
    }
}
