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

    public List<Unit> units;
    public List<Building> buildings;
    public List<TemporaryBuilding> temporaryBuildings;

    private float gold = 50;
    public float Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            OnResourceChange();
        }
    }
    private float wood = 50;
    public float Wood
    {
        get { return wood; }
        set
        {
            wood = value;
            OnResourceChange();
        }
    }

    private float food = 50;
    public float Food
    {
        get { return food; }
        set
        {
            food = value;
            OnResourceChange();
        }
    }

    public Selectable SelectedObject { get; set; }
    public TemporaryBuilding BuildingToBuild { get; private set; }

    public void Start()
    {
        foreach (var player in FindObjectsOfType<Player>())
            if (player.hasAuthority)
                this.player = player;
    }

    public void Select(Selectable selectable)
    {
        if (SelectedObject == selectable)
            return;
        if (SelectedObject != null)
            Deselect();
        SelectedObject = selectable;
        selectable.SetSelection(true, player);
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
        SelectedObject.SetSelection(false, player);

        SelectedObject = null;
    }

    public void OnTransactionLoading(Building building)
    {
        if (building == SelectedObject)
            UIManager.Instance.ShowTransactions(building.transactions);
    }

    public void OnResourceChange()
    {
        UIManager.Instance.ChangeResourceText(GetResourceText());
    }

    private string GetResourceText()
    {
        return string.Format("Food: {0}\nWood: {1}\nGold: {2}", Food, Wood, Gold);
    }

    public void OnStateChange(Selectable selectable)
    {
        if (SelectedObject == selectable)
            UIManager.Instance.ShowObjectText(selectable.Name, selectable.GetObjectDescription());

    }

    public void MoveBuildingToBuild(Vector3 hitPoint)
    {
        // pokud je to misto zabrane, nepohne se
        // TODO
        /*BuildingToBuild.transform.position = GameState.Instance.GetClosestDestination(hitPoint);
        if (GameState.Instance.IsOccupied(BuildingToBuild))
            return;*/
        BuildingToBuild.transform.position = hitPoint;
    }

    public void SetTempBuilding(TemporaryBuilding building)
    {
        BuildingToBuild = building;
    }

    public void PlaceBuilding()
    {
       /* if (GameState.Instance.IsOccupied(BuildingToBuild))
            return;*/
        player.PlaceBuilding(BuildingToBuild);
        ((Commandable)SelectedObject)?.SetGoal(BuildingToBuild);
        BuildingToBuild = null;
    }

    public TemporaryBuilding GetNearestTempBuilding(TemporaryBuilding build, Vector3 position, int maxDistance)
    {
        return temporaryBuildings.Where(b => b != build && Vector3.Distance(position, b.transform.position) < maxDistance).
            OrderBy(b => Vector3.Distance(position, b.transform.position)).FirstOrDefault();
    }
    public bool Pay(int food, int wood, int gold)
    {
        if (Food < food || Wood < wood || Gold < gold)
            return false;
        Food -= food;
        Wood -= wood;
        Gold -= gold;
        return true;
    }

    public void MinimapMove(Vector3 position)
    {
        SelectedObject?.RightMouseClickGround(position);
    }
}
