using System;
using System.Text;

public class Purchase
{
    public string Name;
    public Action action;

    protected int food, wood, gold;
    protected string description;


    public Purchase(string Name, string description, Action action, int food, int wood, int gold)
    {
        this.Name = Name;
        this.food = food;
        this.wood = wood;
        this.gold = gold;
        this.description = description;
        this.action = action;
    }


    public virtual void Do()
    {
        if (PlayerState.Instance.Pay(food, wood, gold))
            action.Invoke();
    }

    public string GetDescription()
    {
        StringBuilder d = new StringBuilder();
        d.AppendLine("<b>" + Name + "</b>");
        if (food > 0)
            d.AppendLine(ResourceDescription("Food", food, PlayerState.Instance.Food));
        if (wood > 0)
            d.AppendLine(ResourceDescription("Wood", wood, PlayerState.Instance.Wood));
        if (gold > 0)
            d.AppendLine(ResourceDescription("Gold", gold, PlayerState.Instance.Gold));
        d.Append(description);
        return d.ToString();
    }

    private string ResourceDescription(string resourceName, int amountToPay, int ownedAmount)
    {
        string line = string.Format("{0}: {1}/{2}", resourceName, amountToPay, ownedAmount);
        if (amountToPay > ownedAmount)
            return "<color=red>" + line + "</color>";
        return line;
    }
}
