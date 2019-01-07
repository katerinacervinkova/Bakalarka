using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldResource : Resource
{
    private static readonly int maxCapacity = 100;
    protected override void DrawNameText()
    {
        nameText.text = "Gold";
    }

    protected override int MaxCapacity()
    {
        return maxCapacity;
    }
}
