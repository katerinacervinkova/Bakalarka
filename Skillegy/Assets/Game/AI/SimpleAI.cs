using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleAI : MonoBehaviour {

    public AIPlayer aiPlayer;

    private LinkedList<Objective> objectives = new LinkedList<Objective>();

    private void Start()
    {
        AddLast(PurchasesEnum.MainBuilding);
        for(int i = 0; i < 10; i++)
            AddLast(PurchasesEnum.Unit);
        AddLast(() => aiPlayer.Explore());
        AddLast(PurchasesEnum.StoneAge);
        AddLast(PurchasesEnum.Barracks);
        for (int i = 0; i < 3; i++)
            AddLast(PurchasesEnum.Library);
        for (int i = 0; i < 10; i++)
            AddLast(() => aiPlayer.TrainUnit(AttEnum.Intelligence));
    }

    void Update () {

        if (objectives.Count == 0)
            return;
        if (objectives.First().function())
        {
            Debug.Log($"food:{aiPlayer.playerState.Food}, wood:{aiPlayer.playerState.Wood}, gold:{aiPlayer.playerState.Gold}, population:{aiPlayer.playerState.Population}/{aiPlayer.playerState.MaxPopulation}");
            objectives.RemoveFirst();
        }
        else
        {
            aiPlayer.CheckPurchaseCost(objectives.First().foodCost, objectives.First().woodCost, objectives.First().goldCost);
            if (aiPlayer.playerState.Population + objectives.First().populationCost > aiPlayer.playerState.MaxPopulation)
            {
                if (aiPlayer.playerState.BuildingToBuild != null && aiPlayer.playerState.BuildingToBuild.buildingType == BuildingEnum.House)
                    return;
                if (aiPlayer.SenseOwnTemporaryBuildings().Where(b => b.buildingType == BuildingEnum.House).Any())
                    return;

                AddFirst(PurchasesEnum.House);
            }
        }
    }

    private void AddLast(Func<bool> function) => objectives.AddLast(new Objective(0, 0, 0, 0, function));
    private void AddFirst(Func<bool> function) => objectives.AddFirst(new Objective(0, 0, 0, 0, function));

    private void AddLast(PurchasesEnum purchasesEnum)
    {
        int food, wood, gold, population;
        aiPlayer.GetPurchaseCost(purchasesEnum, out food, out wood, out gold, out population);
        objectives.AddLast(new Objective(food, wood, gold, population, () => aiPlayer.DoPurchase(purchasesEnum)));
        if ((int)purchasesEnum < 8)
            AddLast(aiPlayer.PlaceBuilding);
    }

    private void AddFirst(PurchasesEnum purchasesEnum)
    {
        if ((int)purchasesEnum < 8)
            AddFirst(aiPlayer.PlaceBuilding);
        int food, wood, gold, population;
        aiPlayer.GetPurchaseCost(purchasesEnum, out food, out wood, out gold, out population);
        objectives.AddFirst(new Objective(food, wood, gold, population, () => aiPlayer.DoPurchase(purchasesEnum)));
    }

    private struct Objective
    {
        public int foodCost;
        public int woodCost;
        public int goldCost;
        public int populationCost;
        public Func<bool> function;

        public Objective(int food, int wood, int gold, int population, Func<bool> function)
        {
            foodCost = food;
            woodCost = wood;
            goldCost = gold;
            populationCost = population;
            this.function = function;
        }
    }
}
