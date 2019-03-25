using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour {

    public Player player;

    public List<Unit> units;
    public List<Building> buildings;
    public List<TemporaryBuilding> temporaryBuildings;

    private int gold = 50;
    public int Gold {
        get { return gold; }
        set {
            gold = value;
            OnResourceChange();
        } }
    private int wood = 50;
    public int Wood
    {
        get { return wood; }
        set
        {
            wood = value;
            OnResourceChange();
        }
    }
    private int food = 50;
    public int Food
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

    public GameState gameState;
    public BottomBar bottomBar;
    public Text nameText;
    public Text selectedObjectText;
    public Text resourceText;

    public void Start()
    {
        foreach (var player in FindObjectsOfType<Player>())
            player.Register(this);
    }

    public void Select(Selectable selectable)
    {
        if (SelectedObject == selectable)
            return;
        if (SelectedObject != null)
            Deselect();
        SelectedObject = selectable;
        selectable.SetSelection(true, player, bottomBar);
        selectable.DrawBottomBar(nameText, selectedObjectText);
        SetUIActive(true);
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
        SelectedObject.RemoveBottomBar(nameText, selectedObjectText);
        SelectedObject.SetSelection(false, player, bottomBar);
        SetUIActive(false);
        SelectedObject = null;
    }

    public void OnResourceChange()
    {
        DrawBottomBar(resourceText);
    }

    private void DrawBottomBar(Text resourceText)
    {
        resourceText.text = String.Format("Food: {0}\nWood: {1}\nGold: {2}", Food, Wood, Gold);
    }

    public void OnStateChange(Selectable selectable)
    {
        if (SelectedObject == selectable)
        {
            selectable.DrawBottomBar(nameText, selectedObjectText);
        }
    }

    private void SetUIActive(bool active)
    {
        if (nameText)
            nameText.gameObject.SetActive(active);
        if (selectedObjectText)
            selectedObjectText.gameObject.SetActive(active);
    }

    public void MoveBuildingToBuild(Vector3 hitPoint)
    {
        // pokud je to misto zabrane, nepohne se
        // TODO
        BuildingToBuild.transform.position = gameState.GetClosestDestination(hitPoint);
        if (gameState.IsOccupied(BuildingToBuild))
            return;
    }

    public void SetWorkerAndBuilding(TemporaryBuilding building)
    {
        BuildingToBuild = building;
    }

    public void PlaceBuilding()
    {
        if (gameState.IsOccupied(BuildingToBuild))
            return;
        player.PlaceBuilding(BuildingToBuild);
        ((Commandable)SelectedObject)?.SetGoal(BuildingToBuild);
        BuildingToBuild = null;
    }

    public bool PayGold(int amount)
    {
        if (Gold < amount)
            return false;
        Gold -= amount;
        return true;
    }
}
