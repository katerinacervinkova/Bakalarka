using UnityEngine.UI;

public class GoldResource : Resource
{
    private static readonly int maxCapacity = 100;
    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        nameText.text = "Gold";
        base.DrawBottomBar(nameText, selectedObjectText);
    }

    protected override int MaxCapacity()
    {
        return maxCapacity;
    }
}
