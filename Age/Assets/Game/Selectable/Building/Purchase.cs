using System;
using System.Text;

public class Purchase
{
    public string Name;
    public Action action;

    protected int food, wood, gold, population;
    protected string description;


    public Purchase(string Name, string description, Action action, int food, int wood, int gold, int population = 0)
    {
        this.Name = Name;
        this.food = food;
        this.wood = wood;
        this.gold = gold;
        this.population = population;
        this.description = description;
        this.action = action;
    }


    public virtual void Do()
    {
        if (PlayerState.Instance.Pay(food, wood, gold, population))
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
        if (gold > 0)
            d.AppendLine(PopulationDescription(population, PlayerState.Instance.Population, PlayerState.Instance.MaxPopulation));
        d.Append(description);
        return d.ToString();
    }

    private string ResourceDescription(string resourceName, int amountToPay, float ownedAmount)
    {
        string line = $"{resourceName}: {amountToPay}/{(int)ownedAmount}";
        if (amountToPay > ownedAmount)
            return $"<color=red>{line}</color>";
        return line;
    }

    private string PopulationDescription(int increase, int current, int max)
    {
        string line = $"Population: {increase}";
        if (increase + current > max)
            return $"<color=red>{line}</color>";
        return line;
    }
}
