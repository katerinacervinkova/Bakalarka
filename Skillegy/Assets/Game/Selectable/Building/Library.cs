using System;
using UnityEngine;

public class Library : Building {

    public override string Name => "Library";

    public override string UnitText(Unit unit)
    {
        switch (Focus)
        {
            // unit description is based on the focus of the library
            case FocusEnum.Intelligence:
                return $"Intelligence: {(int)unit.Intelligence}";
            case FocusEnum.Building:
                return $"Building: {(int)unit.Building}";
            case FocusEnum.Healing:
                return $"Healing: {(int)unit.Healing}";
            default:
                return "";
        }
    }

    public int maxIntelligence = 10;

    // three states of the library
    public enum FocusEnum { Intelligence, Building, Healing }
    public FocusEnum Focus = FocusEnum.Intelligence;

    // true if the corresponding purchase has been already made
    public bool Books1 = false;
    public bool Books2 = false;
    public bool Books3 = false;
    public bool Books4 = false;
    public bool Books5 = false;

    private readonly float buildingIncrease = 0.005f;
    private readonly float slowBuildingIncrease = 0.001f;
    private readonly float healingIncrease = 0.005f;
    private readonly float slowHealingIncrease = 0.001f;
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

    /// <summary>
    /// Changes unit's skill level based on the library's focus.
    /// </summary>
    protected override void UpdateUnit(Unit unit)
    {
        switch (Focus)
        {
            case FocusEnum.Intelligence:
                if (unit.Intelligence < maxIntelligence)
                    owner.ChangeAttribute(unit, SkillEnum.Intelligence, Math.Min(maxIntelligence, unit.Intelligence + intelligenceIncrease));
                else
                    owner.ChangeAttribute(unit, SkillEnum.Intelligence, unit.Intelligence + slowIntelligenceIncrease);
                break;
            case FocusEnum.Building:
                if (unit.Building < unit.Intelligence)
                    owner.ChangeAttribute(unit, SkillEnum.Building, unit.Building + buildingIncrease * unit.Intelligence);
                else
                    owner.ChangeAttribute(unit, SkillEnum.Building, unit.Building + slowBuildingIncrease * unit.Intelligence);
                break;
            case FocusEnum.Healing:
                if (unit.Healing < unit.Intelligence)
                    owner.ChangeAttribute(unit, SkillEnum.Healing, unit.Healing + healingIncrease * unit.Intelligence);
                else
                    owner.ChangeAttribute(unit, SkillEnum.Healing, unit.Healing + slowHealingIncrease * unit.Intelligence);
                break;
        }

    }
}
