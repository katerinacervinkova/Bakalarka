﻿using System;
using UnityEngine;

public class Barracks : Building
{
    public override string Name => "Barracks";
    public override string UnitText(Unit unit) => $"Swordsmanship: {(int)unit.Swordsmanship}";

    // true if the corresponding purchase has been already made.
    public bool Gear1;
    public bool Gear2;
    public bool Gear3;
    public bool Gear4;
    public bool Gear5;

    public int maxSwordsmanship = 10;
    private readonly float swordsmanshipIncrease = 0.005f;
    private readonly float slowSwordsmanshipIncrease = 0.001f;

    protected override void ChangeColor()
    {
        visibleObject.transform.Find("Building/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
        visibleObject.transform.Find("Building/Dog").GetComponent<MeshRenderer>().material.color = owner.color;
    }

    /// <summary>
    /// Increases unit's Swordsmanship level.
    /// </summary>
    protected override void UpdateUnit(Unit unit)
    {
        if (unit.Swordsmanship < maxSwordsmanship)
            owner.ChangeAttribute(unit, SkillEnum.Swordsmanship, Math.Min(maxSwordsmanship, unit.Swordsmanship + swordsmanshipIncrease * unit.Intelligence));
        else
            owner.ChangeAttribute(unit, SkillEnum.Swordsmanship, unit.Swordsmanship + slowSwordsmanshipIncrease * unit.Intelligence);
    }

    protected override void InitPurchases()
    {
        AddPurchase(PurchasesEnum.Gear1);
        AddPurchase(PurchasesEnum.Gear2);
        AddPurchase(PurchasesEnum.Gear3);
        AddPurchase(PurchasesEnum.Gear4);
        AddPurchase(PurchasesEnum.Gear5);
    }
}
