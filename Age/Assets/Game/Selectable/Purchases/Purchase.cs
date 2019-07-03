using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Purchase
{
    protected readonly int playerId;

    public readonly string Name;
    public readonly Texture2D image;
    protected readonly Action<Selectable> action;
    public readonly Predicate<Selectable> IsActive;
    private Dictionary<Selectable, bool> wasActive = new Dictionary<Selectable, bool>();

    protected int food, wood, gold, population;
    protected string description;

    public Purchase(string Name, int playerId, Texture2D image, string description, Action<Selectable> action, Predicate<Selectable> IsActive,
        int food, int wood, int gold, int population = 0)
    {
        this.Name = Name;
        this.playerId = playerId;
        this.food = food;
        this.wood = wood;
        this.gold = gold;
        this.population = population;
        this.description = description;
        this.action = action;
        this.IsActive = IsActive;
        this.image = image;
    }


    public virtual bool Do(Selectable selectable)
    {
        if (PlayerState.Get(playerId).Pay(food, wood, gold, population))
        {
            action.Invoke(selectable);
            return true;
        }
        return false;
    }

    public virtual void Reset(Selectable selectable = null)
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
        if (population > 0)
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

    public bool ActiveChanged(Selectable selectable)
    {
        bool active = IsActive(selectable);

        if (!wasActive.ContainsKey(selectable)) 
            wasActive[selectable] = active;
        if (active == wasActive[selectable])
            return false;
        wasActive[selectable] = active;
        return true;
    }
}
