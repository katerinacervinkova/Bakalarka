using System;
using UnityEngine;

public class Library : Building {

    public override string Name => "Library";

    public enum FocusEnum { Intelligence, Building, Healing }

    public FocusEnum Focus = FocusEnum.Intelligence;

    public bool Books1 = false;
    public bool Books2 = false;
    public bool Books3 = false;
    public bool Books4 = false;
    public bool Books5 = false;

    public override Func<Unit, string> UnitTextFunc
    {
        get
        {
            switch (Focus)
            {
                case FocusEnum.Intelligence:
                    return u => $"Intelligence: {(int)u.Intelligence}";
                case FocusEnum.Building:
                    return u => $"Building: {(int)u.Building}";
                case FocusEnum.Healing:
                    return u => $"Healing: {(int)u.Healing}";
                default:
                    return u => "";
            }
        }
    }

    public int maxIntelligence = 10;

    private readonly float buildingIncrease = 0.005f;
    private readonly float healingIncrease = 0.005f;
    private readonly float intelligenceIncrease = 0.1f;
    private readonly float slowIntelligenceIncrease = 0.02f;

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    protected override void InitPurchases()
    {
        AddPurchase(PurchasesEnum.Books1);
        AddPurchase(PurchasesEnum.Books2);
        AddPurchase(PurchasesEnum.Books3);
        AddPurchase(PurchasesEnum.Books4);
        AddPurchase(PurchasesEnum.Books5);
        AddPurchase(PurchasesEnum.BuildingBooks);
        AddPurchase(PurchasesEnum.MedicineBooks1);
        AddPurchase(PurchasesEnum.MedicineBooks2);
        AddPurchase(PurchasesEnum.Building);
        AddPurchase(PurchasesEnum.Healing);
        AddPurchase(PurchasesEnum.Intelligence);
    }

    protected override void UpdateUnit(Unit unit)
    {
        switch (Focus)
        {
            case FocusEnum.Intelligence:
                if (unit.Intelligence < maxIntelligence)
                    owner.ChangeAttribute(unit, AttEnum.Intelligence, Math.Min(maxIntelligence, unit.Intelligence + intelligenceIncrease));
                else
                    owner.ChangeAttribute(unit, AttEnum.Intelligence, unit.Intelligence + slowIntelligenceIncrease);
                break;
            case FocusEnum.Building:
                owner.ChangeAttribute(unit, AttEnum.Building, unit.Building + buildingIncrease * unit.Intelligence);
                break;
            case FocusEnum.Healing:
                owner.ChangeAttribute(unit, AttEnum.Healing, unit.Healing + healingIncrease * unit.Intelligence);
                break;
        }

    }
}
