using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerState : MonoBehaviour {

    private static readonly PlayerState[] instances = new PlayerState[6];

    public static PlayerState[] GetAll() => instances;
    public static PlayerState Get(int i) => instances[i];
    public static PlayerState Get() => Get(0);
    public static void Set(int i, PlayerState value) => instances[i] = value;

    public Player player;
    public PlayerPurchases playerPurchases;

    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();
    public List<TemporaryBuilding> temporaryBuildings = new List<TemporaryBuilding>();

    public enum AgeEnum { Wood, Stone, Iron, Diamond }
    private AgeEnum age = AgeEnum.Wood;

    private float food = 1200;
    private float wood = 1200;
    private float gold = 1200;
    private int population = 0;
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
    public int Population
    {
        get { return population; }
        set
        {
            population = value;
            OnPlayerStateChange();
        }
    }
    public float Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            OnPlayerStateChange();
        }
    }
    public float Wood
    {
        get { return wood; }
        set
        {
            wood = value;
            OnPlayerStateChange();
        }
    }
    public float Food
    {
        get { return food; }
        set
        {
            food = value;
            OnPlayerStateChange();
        }
    }
    public AgeEnum Age
    {
        get { return age; }
        set
        {
            age = value;
            OnPlayerStateChange();
        }
    }

    public bool BuildingBooks { get; set; } = false;
    public bool MedicineBooks1 { get; set; } = false;
    public bool MedicineBooks2 { get; set; } = false;


    public Selectable SelectedObject { get; private set; }
    public TemporaryBuilding BuildingToBuild { get; private set; }

    public void Select(Selectable selectable)
    {
        if (SelectedObject == selectable)
            return;
        if (SelectedObject != null)
            Deselect();
        SelectedObject = selectable;
        if (player.IsHuman)
        {
            selectable.SetSelection(true);
            UIManager.Instance.ShowObjectText(selectable.Name, selectable.GetObjectDescription());
        }
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
        if (player.IsHuman)
        {
            UIManager.Instance?.HideObjectText();
            SelectedObject?.SetSelection(false);
        }

        SelectedObject = null;
    }

    public void OnTransactionLoading(Building building)
    {
        if (player.IsHuman && building == SelectedObject)
            UIManager.Instance.ShowTransactions(building.transactions);
    }

    public void OnPlayerStateChange()
    {
        if (player != null && player.IsHuman && UIManager.Instance != null)
            UIManager.Instance.ChangePlayerStateText(player.Name, Age, GetResourceText());
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

    public void SelectIdle()
    {
        List<Unit> idleUnits = IdleUnits();
        var rnd = new System.Random();
        if (idleUnits.Count > 0)
            Select(idleUnits[rnd.Next(idleUnits.Count)]);
    }

    public List<Unit> IdleUnits() => units.Where(u => !u.HasJob).ToList();

    public Unit BestUnit(AttEnum attribute) => units.OrderByDescending(u => u.GetAttribute(attribute)).FirstOrDefault();
    public Unit BestIdleUnit(AttEnum attribute) => IdleUnits().OrderByDescending(u => u.GetAttribute(attribute)).FirstOrDefault();

    public List<Unit> GoodUnits(AttEnum attribute, float bar) => units.Where(u => u.GetAttribute(attribute) > bar).ToList();
    public List<Unit> GoodIdleUnits(AttEnum attribute, float bar) => IdleUnits().Where(u => u.GetAttribute(attribute) > bar).ToList();

    public void OnStateChange(Selectable selectable)
    {
        if (player.IsHuman && SelectedObject == selectable)
        {
            UIManager.Instance.ShowObjectText(selectable.Name, selectable.GetObjectDescription());
            if (selectable.playerId == 0)
                UIManager.Instance.ShowPurchaseButtons(selectable.Purchases, selectable);
        }
    }

    public void OnStateChange(Purchase purchase)
    {
        if (SelectedObject != null && SelectedObject.Purchases.Contains(purchase) && SelectedObject.playerId == 0)
            UIManager.Instance.ShowPurchaseButtons(SelectedObject.Purchases, SelectedObject);
    }

    public bool MoveBuildingToBuild(Vector3 hitPoint)
    {
        var bounds = BuildingToBuild.Bounds;
        bounds.center = hitPoint;
        foreach (var node in AstarPath.active.data.gridGraph.GetNodesInRegion(bounds))
            if (!node.Walkable)
                return false;
        BuildingToBuild.transform.position = hitPoint;
        return true;
    }

    public void SetTempBuilding(TemporaryBuilding building)
    {
        if (BuildingToBuild != null)
            ResetBuildingToBuild();
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

    public TemporaryBuilding GetNearestTempBuilding(TemporaryBuilding build, Vector3 position, int maxDistance) => 
        temporaryBuildings.Where(b => b != build && Vector3.Distance(position, b.transform.position) < maxDistance).
        OrderBy(b => Vector3.Distance(position, b.transform.position)).FirstOrDefault();

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
        if (player.IsHuman)
            SelectedObject?.RightMouseClickGround(position);
    }
}
