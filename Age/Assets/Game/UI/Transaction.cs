using System;
using System.Text;

public class Transaction
{
    public string Name;
    public Action action;

    private readonly int food, wood, gold;
    private string description;


    public Transaction(string Name, string description, Action action, int food, int wood, int gold)
    {
        this.Name = Name;
        this.food = food;
        this.wood = wood;
        this.gold = gold;
        this.description = description;
        this.action = action;
    }


    public void Do()
    {
        if (PlayerState.Instance.Pay(food, wood, gold))
            action.Invoke();
    }

    public string GetDescription()
    {
        StringBuilder d = new StringBuilder(Name + "\n");
        if (food > 0)
            d.AppendFormat("Food: {0}/{1}\n", food, PlayerState.Instance.Food);
        if (wood > 0)
            d.AppendFormat("Wood: {0}/{1}\n", wood, PlayerState.Instance.Wood);
        if (gold > 0)
            d.Append("Gold: {0}/{1}\n", gold, PlayerState.Instance.Gold);
        d.Append(description);
        return d.ToString();
    }
}
