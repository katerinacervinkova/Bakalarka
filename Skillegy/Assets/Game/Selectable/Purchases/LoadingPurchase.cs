using System;
using UnityEngine;

public class LoadingPurchase : Purchase {

    private readonly float Speed;
    private readonly bool oneTimePurchase;

	public LoadingPurchase(float Speed, string Name, int playerId, Texture2D image, string description, Action<Selectable> action, Predicate<Selectable> activeCondition,
        int food, int wood, int gold, int population = 0, bool oneTimePurchase = false)
        : base(Name, playerId, image, description, action, activeCondition, food, wood, gold, population)
    {
        this.Speed = Speed;
        this.oneTimePurchase = oneTimePurchase;
    }

    public override bool Do(Selectable selectable)
    {
        Building building = selectable as Building;
        if (building.CanStartTransaction() && PlayerState.Get(playerId).Pay(food, wood, gold, population))
        {
            if (oneTimePurchase)
            {
                building.RemovePurchase(this);
                if (playerId == 0)
                    UIManager.Instance.HideToolTip();
            }
            building.AddTransaction(new Transaction(building, this, Speed));
            return true;
        }
        return false;
    }

    public void InvokeAction(Selectable selectable)
    {
        action.Invoke(selectable);
    }

    public override void Reset(Selectable selectable = null)
    {
        base.Reset();
        if (oneTimePurchase)
            (selectable as Building).AddPurchase(this);
    }
}
