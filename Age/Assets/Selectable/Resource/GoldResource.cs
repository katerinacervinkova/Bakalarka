﻿using System;
using UnityEngine.UI;

public class GoldResource : Resource
{
    private static readonly int maxCapacity = 100;

    protected override int MaxCapacity => maxCapacity;

    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        nameText.text = "Gold";
        base.DrawBottomBar(nameText, selectedObjectText);
    }

    public override void Mine(Unit worker)
    {
        int amount = Math.Min(worker.Strength, capacity);
        worker.owner.Mine(amount, this);
        playerState.Gold += amount;
    }
}
