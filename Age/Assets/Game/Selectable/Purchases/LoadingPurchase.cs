﻿using System;
using UnityEngine;

public class LoadingPurchase : Purchase {

    private readonly float Speed;

	public LoadingPurchase(float Speed, string Name, int playerId, Texture2D image, string description, Action<Selectable> action, int food, int wood, int gold, int population = 0)
        : base(Name, playerId, image, description, action, food, wood, gold, population)
    {
        this.Speed = Speed;
    }

    public override bool Do(Selectable selectable)
    {
        Building building = selectable as Building;
        if (building.CanStartTransaction() && PlayerState.Get(playerId).Pay(food, wood, gold, population))
        {
            building.AddTransaction(new Transaction(building, this, Speed));
            return true;
        }
        return false;
    }

    public void InvokeAction(Selectable selectable)
    {
        action.Invoke(selectable);
    }
}
