using System;
using System.Text;
using UnityEngine;

public class Purchase
{
    protected readonly int playerId;

    public string Name;
    public Texture2D image;
    public Action<Selectable> action;

    protected int food, wood, gold, population;
    protected string description;


    public Purchase(string Name, int playerId, Texture2D image, string description, Action<Selectable> action, int food, int wood, int gold, int population = 0)
    {
        this.Name = Name;
        this.playerId = playerId;
        this.food = food;
        this.wood = wood;
        this.gold = gold;
        this.population = population;
        this.description = description;
        this.action = action;
        this.image = image;
    }


    public virtual void Do(Selectable selectable)
    {
        if (PlayerState.Get(playerId).Pay(food, wood, gold, population))
            action.Invoke(selectable);
    }

    public void Reset()
    {
        PlayerState.Get(playerId).Pay(-food, -wood, -gold, -population);
    }

    public string GetDescription()
    {
        StringBuilder d = new StringBuilder();
        d.AppendLine("<b>" + Name + "</b>");
        if (food > 0)
            d.AppendLine(ResourceDescription("Food", food, PlayerState.Get(playerId).Food));
        if (wood > 0)
            d.AppendLine(ResourceDescription("Wood", wood, PlayerState.Get(playerId).Wood));
        if (gold > 0)
            d.AppendLine(ResourceDescription("Gold", gold, PlayerState.Get(playerId).Gold));
        if (gold > 0)
            d.AppendLine(PopulationDescription(population, PlayerState.Get(playerId).Population, PlayerState.Get(playerId).MaxPopulation));
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
