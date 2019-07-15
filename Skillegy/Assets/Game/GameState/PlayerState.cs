using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Player's GameState that doesn't need to be synchronized.
/// </summary>
public class PlayerState : MonoBehaviour {

    //one instance for each player using the same client
    private static readonly PlayerState[] instances = new PlayerState[6];

    public static PlayerState[] GetAll() => instances;
    public static PlayerState Get(int i) => instances[i];
    public static PlayerState Get() => Get(0);
    public static void Set(int i, PlayerState value) => instances[i] = value;

    public Player player;
    public PlayerPurchases playerPurchases;

    // player's property
    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();
    public List<TemporaryBuilding> temporaryBuildings = new List<TemporaryBuilding>();

    // age the player is currently in
    public enum AgeEnum { Wood, Stone, Iron, Diamond }
    private AgeEnum age = AgeEnum.Wood;

    // player's resources
    private float food = 1200;
    private float wood = 1200;
    private float gold = 1200;
    private int population = 0;
    private int maxPopulation = 0;

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

    // true if this technological advancement has been made
    public bool BuildingBooks { get; set; } = false;
    public bool MedicineBooks1 { get; set; } = false;
    public bool MedicineBooks2 { get; set; } = false;

    // currently selected object
    public Selectable SelectedObject { get; private set; }
    // building the player is currently placing
    public TemporaryBuilding BuildingToBuild { get; private set; }

    /// <summary>
    /// Select given object.
    /// </summary>
    /// <param name="selectable">object to be selected</param>
    public void Select(Selectable selectable)
    { 
        if (SelectedObject == selectable)
            return;
        if (SelectedObject != null)
            Deselect();
        SelectedObject = selectable;
        // show selected object UI for human player
        if (player.IsHuman)
        {
            selectable.SetSelection(true);
            UIManager.Instance.ShowObjectText(selectable.Name, selectable.GetObjectDescription());
        }
    }

    /// <summary>
    /// Select all player's units that correspond to the given predicate.
    /// </summary>
    /// <param name="predicate">predicate according to which units are chosen</param>
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

    /// <summary>
    /// Deselect any currently selected object.
    /// </summary>
    public void Deselect()
    {
        if (player.IsHuman)
        {
            UIManager.Instance?.HideObjectText();
            SelectedObject?.SetSelection(false);
        }

        SelectedObject = null;
    }

    /// <summary>
    /// Show the progress of transactions inside given building.
    /// </summary>
    /// <param name="building">building whose transaction are to be shown</param>
    public void OnTransactionLoading(Building building)
    {
        if (player.IsHuman && building == SelectedObject)
            UIManager.Instance.ShowTransactions(building.transactions);
    }

    /// <summary>
    /// Update player UI after a change of its state.
    /// </summary>
    public void OnPlayerStateChange()
    {
        if (player != null && player.IsHuman && UIManager.Instance != null)
            UIManager.Instance.ChangePlayerStateText(player.Name, Age, GetResourceText());
    }

    /// <summary>
    /// Returns text describing current state of player's resources.
    /// </summary>
    /// <returns>text describing current state of player's resources.</returns>
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

    /// <summary>
    /// Selects random unit which has nothing to do.
    /// </summary>
    public void SelectIdle()
    {
        List<Unit> idleUnits = IdleUnits();
        var rnd = new System.Random();
        if (idleUnits.Count > 0)
            Select(idleUnits[rnd.Next(idleUnits.Count)]);
    }

    /// <summary>
    /// Gets units that have nothing to do.
    /// </summary>
    /// <returns>list of units that have nothing to do</returns>
    public List<Unit> IdleUnits() => units.Where(u => !u.HasJob).ToList();

    /// <summary>
    /// Returns best unit based on the level of the given skill.
    /// </summary>
    public Unit BestUnit(SkillEnum skill) => units.OrderByDescending(u => u.GetSkillLevel(skill)).FirstOrDefault();
    /// <summary>
    /// Returns best unit based on the level of the given skill which has nothing to do.
    /// </summary>
    public Unit BestIdleUnit(SkillEnum skill) => IdleUnits().OrderByDescending(u => u.GetSkillLevel(skill)).FirstOrDefault();

    /// <summary>
    /// Returns unit which has level of given skill better the given bar.
    /// </summary>
    public List<Unit> GoodUnits(SkillEnum skill, float bar) => units.Where(u => u.GetSkillLevel(skill) > bar).ToList();
    /// <summary>
    /// Returns unit which has level of given skill better the given bar and has nothing to do.
    /// </summary>
    public List<Unit> GoodIdleUnits(SkillEnum skill, float bar) => IdleUnits().Where(u => u.GetSkillLevel(skill) > bar).ToList();

    /// <summary>
    /// Called when the state of the selectable has changed. Updates UI if the selectable is currently selected and the player is human.
    /// </summary>
    /// <param name="selectable">selectable whose state has just changed</param>
    public void OnStateChange(Selectable selectable)
    {
        if (player.IsHuman && SelectedObject == selectable)
        {
            UIManager.Instance.ShowObjectText(selectable.Name, selectable.GetObjectDescription());
            if (selectable.playerId == 0)
                UIManager.Instance.ShowPurchaseButtons(selectable.Purchases, selectable);
        }
    }

    /// <summary>
    /// Called when purchase becomes active or inactive. Updates UI if the corresponding selectable is currently selected.
    /// </summary>
    /// <param name="purchase">purchase whose state has just changed</param>
    public void OnStateChange(Purchase purchase)
    {
        if (SelectedObject != null && SelectedObject.Purchases.Contains(purchase) && SelectedObject.playerId == 0)
            UIManager.Instance.ShowPurchaseButtons(SelectedObject.Purchases, SelectedObject);
    }

    /// <summary>
    /// Moves the building which is currently being placed to given position if the position is not occupied.
    /// </summary>
    /// <param name="position">position to place the building</param>
    /// <returns>true if the moving was succeeded</returns>
    public bool MoveBuildingToBuild(Vector3 position)
    {
        var bounds = BuildingToBuild.Bounds;
        bounds.center = position;
        foreach (var node in AstarPath.active.data.gridGraph.GetNodesInRegion(bounds))
            if (!node.Walkable)
                return false;
        BuildingToBuild.transform.position = position;
        return true;
    }

    /// <summary>
    /// Sets the new building to be placed. If there already is one, resets the old one's purchase.
    /// </summary>
    /// <param name="building">the new building to build</param>
    public void SetTempBuilding(TemporaryBuilding building)
    {
        if (BuildingToBuild != null)
            ResetBuildingToBuild();
        BuildingToBuild = building;
    }

    /// <summary>
    /// Resets the building's purchase by returning paid resources and destroying the object.
    /// </summary>
    public void ResetBuildingToBuild()
    {
        playerPurchases.Get(BuildingToBuild.buildingType).Reset();
        player.DestroySelectedObject(BuildingToBuild);
        BuildingToBuild = null;
    }

    /// <summary>
    /// Place the building on its current position and tells the selected object to build it.
    /// </summary>
    public void PlaceBuilding()
    {
        player.PlaceBuilding(BuildingToBuild);
        if (SelectedObject != null)
            SelectedObject.SetGoal(BuildingToBuild);
        BuildingToBuild = null;
    }

    /// <summary>
    /// Get the nearest temporary building to the given position in the given range, which is not a given building.
    /// </summary>
    /// <param name="build">building to be excluded from the list</param>
    /// <param name="position">center of the search</param>
    /// <param name="maxDistance">maximum radius</param>
    /// <returns></returns>
    public TemporaryBuilding GetNearestTempBuilding(TemporaryBuilding build, Vector3 position, int maxDistance) => 
        temporaryBuildings.Where(b => b != build && Vector3.Distance(position, b.transform.position) < maxDistance).
        OrderBy(b => Vector3.Distance(position, b.transform.position)).FirstOrDefault();

    /// <summary>
    /// Pay the given amount of resources from the player's fund.
    /// </summary>
    public bool Pay(int food, int wood, int gold, int population)
    {
        if (Food < food || Wood < wood || Gold < gold || (population > 0 && Population + population > MaxPopulation))
            return false;
        this.food -= food;
        this.wood -= wood;
        this.gold -= gold;
        this.population += population;
        OnPlayerStateChange();
        return true;
    }

    /// <summary>
    /// Sends the currently selected object to the given object.
    /// </summary>
    public void RightClickMinimap(Vector3 position)
    {
        if (player.IsHuman)
            SelectedObject?.SetGoal(position);
    }
}
