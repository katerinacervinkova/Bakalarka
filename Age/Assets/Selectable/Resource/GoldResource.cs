using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldResource : Resource
{
    private static readonly int maxCapacity = 100;
    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        nameText.text = "Gold";
    }

    protected override int MaxCapacity()
    {
        return maxCapacity;
    }
}
