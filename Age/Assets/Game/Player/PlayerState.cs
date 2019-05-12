using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerState : MonoBehaviour {

    private static PlayerState instance;
    public static PlayerState Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerState>();
            return instance;
        }
    }

    public Player player;
    public PlayerPurchases playerPurchases;

    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();
    public List<TemporaryBuilding> temporaryBuildings = new List<TemporaryBuilding>();

    private int maxPopulation = 5;
    public int MaxPopulation
    {
        get { return maxPopulation; }
        set
        {
            maxPopulation = value;
            OnPlayerStateChange();
        }
    }



    private int population = 5;
    public int Population
    {
        get { return population; }
        set
        {
            population = value;
            OnPlayerStateChange();
        }
    }
    private float gold = 50;
    public float Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            OnPlayerStateChange();
        }
    }
    private float wood = 50;
    public float Wood
    {
        get { return wood; }
        set
        {
            wood = value;
            OnPlayerStateChange();
        }
    }

    private float food = 50;
    public float Food
    {
        get { return food; }
        set
        {
            food = value;
            OnPlayerStateChange();
        }
    }

    public Selectable SelectedObject { get; set; }
    public TemporaryBuilding BuildingToBuild { get; private set; }

    public void Select(Selectable selectable)
    {
        if (SelectedObject == selectable)
            return;
        if (SelectedObject != null)
            Deselect();
        SelectedObject = selectable;
        selectable.SetSelection(true);
        UIManager.Instance.ShowObjectText(selectable.Name, selectable.GetObjectDescription());
    }

    public void Select(Predicate<Unit> predicate)
    {
        var u = units.FindAll(predicate);
        if (u.Count == 0)
            return;
        if (u.Count == 1)
            Select(u[0]);
        else
            Select(player.factory.CreateRegiment(player, u));
    }

    public void Deselect()
    {
        UIManager.Instance?.HideObjectText();
        SelectedObject.SetSelection(false);

        SelectedObject = null;
    }

    public void OnTransactionLoading(Building building)
    {
        if (building == SelectedObject)
            UIManager.Instance.ShowTransactions(building.transactions);
    }

    public void OnPlayerStateChange()
    {
        if (UIManager.Instance != null && player != null)
            UIManager.Instance.ChangePlayerStateText(player.Name, GetResourceText());
    }

    public bool IsWithinSight(Vector3 position)
    {
        return units.Any(u => u.IsWithinSight(position)) ||
            buildings.Any(u => u.IsWithinSight(position)) ||
            temporaryBuildings.Any(t => t.IsWithinSight(position));
    }

    private string GetResourceText()
    {
        string colorStart = "";
        string colorEnd = "";
        if (Population > MaxPopulation)
        {
            colorStart = "<color=red>";
            colorEnd = "</color>";
        }
        return $"Food: {(int)Food}\n" +
            $"Wood: {(int)Wood}\n" +
            $"Gold: {(int)Gold}\n" +
            $"{colorStart}Population: {Population}/{MaxPopulation}{colorEnd}";
    }

    public void OnStateChange(Selectable selectable)
    {
        if (SelectedObject == selectable)
            UIManager.Instance.ShowObjectText(selectable.Name, selectable.GetObjectDescription());
    }

    public void MoveBuildingToBuild(Vector3 hitPoint)
    {
        var bounds = BuildingToBuild.bounds;
        bounds.center = hitPoint;
        foreach (var node in AstarPath.active.data.gridGraph.GetNodesInRegion(bounds))
            if (!node.Walkable)
                return;
        BuildingToBuild.transform.position = hitPoint;
    }

    public void SetTempBuilding(TemporaryBuilding building)
    {
        BuildingToBuild = building;
    }

    public void ResetBuildingToBuild()
    {
        playerPurchases.Get(BuildingToBuild.buildingType).Reset();
        player.DestroySelectedObject(BuildingToBuild);
        BuildingToBuild = null;
    }

    public void PlaceBuilding()
    {
        player.PlaceBuilding(BuildingToBuild);
        ((Commandable)SelectedObject)?.SetGoal(BuildingToBuild);
        BuildingToBuild = null;
    }

    public TemporaryBuilding GetNearestTempBuilding(TemporaryBuilding build, Vector3 position, int maxDistance)
    {
        return temporaryBuildings.Where(b => b != build && Vector3.Distance(position, b.transform.position) < maxDistance).
            OrderBy(b => Vector3.Distance(position, b.transform.position)).FirstOrDefault();
    }
    public bool Pay(int food, int wood, int gold, int population)
    {
        if (Food < food || Wood < wood || Gold < gold || Population + population > MaxPopulation)
            return false;
        this.food -= food;
        this.wood -= wood;
        this.gold -= gold;
        this.population += population;
        OnPlayerStateChange();
        return true;
    }

    public void MinimapMove(Vector3 position)
    {
        SelectedObject?.RightMouseClickGround(position);
    }
}
