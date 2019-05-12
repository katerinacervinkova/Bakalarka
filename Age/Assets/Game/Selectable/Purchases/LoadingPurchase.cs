using System;
using UnityEngine;

public class LoadingPurchase : Purchase {

    private readonly float Speed;

	public LoadingPurchase(float Speed, string Name, Texture2D image, string description, Action<Selectable> action, int food, int wood, int gold, int population = 0)
        : base(Name, image, description, action, food, wood, gold, population)
    {
        this.Speed = Speed;
    }

    public override void Do(Selectable selectable)
    {
        Building building = selectable as Building;
        if (building.CanStartTransaction() && PlayerState.Instance.Pay(food, wood, gold, population))
                building.AddTransaction(new Transaction(building, this, Speed));
    }

    public void InvokeAction(Selectable selectable)
    {
        action.Invoke(selectable);
    }
}
