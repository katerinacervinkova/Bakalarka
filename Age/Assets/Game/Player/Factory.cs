using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Factory : MonoBehaviour
{
    protected System.Random rnd;
    protected int sumOfProperties = 35;

    public GameState gameState;
    public TemporaryBuilding temporaryBuildingPrefab;
    public Building mainBuildingPrefab;
    public Regiment regimentPrefab;
    public Unit unitPrefab;
    public Resource goldPrefab;
    public Scheduler schedulerPrefab;
    public BottomBar bottomBar;

    public Image panel;
    protected Text selectedAttributesText;
    protected Text nameText;

    private void Awake()
    {
        bottomBar = GameObject.Find("Panel").GetComponent<BottomBar>();
    }
    protected virtual void Start()
    {
        regimentPrefab.gameObject.SetActive(false);
        unitPrefab.gameObject.SetActive(false);
        schedulerPrefab.gameObject.SetActive(false);
        rnd = new System.Random();
        panel = GameObject.Find("Panel").GetComponent<Image>();
        selectedAttributesText = panel.transform.Find("selectableAttributesText").GetComponent<Text>();
        nameText = panel.transform.Find("nameText").GetComponent<Text>();
    }
   
    public Regiment CreateRegiment(Player owner, List<Unit> units)
    {
        Regiment regiment = Instantiate(regimentPrefab);
        regiment.owner = owner;
        regiment.SetGameState(gameState);
        regiment.SetUnits(units);
        regiment.Name = string.Format("Units({0})", units.Count);
        regiment.gameObject.SetActive(true);
        return regiment;
    }

    public Unit CreateUnit(Vector3 spawnPoint, NetworkInstanceId playerId)
    {
        Unit unit = Instantiate(unitPrefab, spawnPoint, Quaternion.identity);
        unit.Name = "Unit";
        unit.playerID = playerId;
        SetRandomParameters(unit);
        unit.gameObject.SetActive(true);
        return unit;
    }

    public TemporaryBuilding CreateTemporaryMainBuilding(NetworkInstanceId playerId)
    {
        TemporaryBuilding building = Instantiate(temporaryBuildingPrefab, Vector3.zero, Quaternion.identity);
        building.playerID = playerId;
        return building;
    }

    public Building CreateMainBuilding(TemporaryBuilding tempBuilding, NetworkInstanceId playerId)
    {
        return CreateMainBuilding(tempBuilding.transform.position, playerId);
    }

    private Building CreateMainBuilding(Vector3 position, NetworkInstanceId playerId)
    {
        Building building = Instantiate(mainBuildingPrefab, position, Quaternion.identity);

        building.name = "Building";
        building.playerID = playerId;
        building.gameObject.SetActive(true);

        return building;
    }

    public Resource CreateGold(Vector3 position)
    {
        Resource gold = Instantiate(goldPrefab, position, Quaternion.identity);

        gold.name = "Gold";
        gold.gameObject.SetActive(true);

        return gold;
    }

    public Scheduler CreateScheduler(Action action, Image image)
    {
        Vector3 position = new Vector3(-300, -30, 0);
        Scheduler scheduler = Instantiate(schedulerPrefab, position, Quaternion.identity);
        scheduler.transform.SetParent(panel.transform, false);
        scheduler.ActionToPerform = action;
        scheduler.image = image;
        scheduler.Speed = 1f / 1;

        return scheduler;
    }

    private void SetRandomParameters(Unit unit)
    {
        int health = rnd.Next(100);
        int strength = rnd.Next(100);
        int intelligence = rnd.Next(100);
        int agility = rnd.Next(100);
        int healing = rnd.Next(100);
        int crafting = rnd.Next(100);
        int accuracy = rnd.Next(100);

        int ratio = (health + strength + intelligence + agility + healing + crafting + accuracy);

        unit.MaxHealth = 100 + health * sumOfProperties / ratio + 1;
        unit.Health = unit.MaxHealth;
        unit.Strength = strength * sumOfProperties / ratio + 1;
        unit.Intelligence = intelligence * sumOfProperties / ratio + 1;
        unit.Agility = agility * sumOfProperties / ratio + 1;
        unit.Healing = healing * sumOfProperties / ratio + 1;
        unit.Crafting = crafting * sumOfProperties / ratio + 1;
        unit.Accuracy = accuracy * sumOfProperties / ratio + 1;
    }
}
