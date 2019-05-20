using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Factory : MonoBehaviour
{
    protected System.Random rnd = new System.Random();
    protected int sumOfProperties = 35;

    public Player player;
    [SerializeField]
    private PlayerPurchases playerPurchasesPrefab;
    [SerializeField]
    private TemporaryBuilding mainBuildingPrefab;
    [SerializeField]
    private TemporaryBuilding libraryPrefab;
    [SerializeField]
    private TemporaryBuilding barracksPrefab;
    [SerializeField]
    private TemporaryBuilding infirmaryPrefab;
    [SerializeField]
    private TemporaryBuilding housePrefab;
    [SerializeField]
    private TemporaryBuilding millPrefab;
    [SerializeField]
    private Regiment regimentPrefab;
    [SerializeField]
    private Unit unitPrefab;
   
    public Regiment CreateRegiment(Player owner, List<Unit> units)
    {
        Regiment regiment = Instantiate(regimentPrefab);
        regiment.owner = owner;
        regiment.SetUnits(units);
        regiment.gameObject.SetActive(true);
        return regiment;
    }

    public Unit CreateUnit(Vector3 spawnPoint, NetworkInstanceId playerId)
    {
        Unit unit = Instantiate(unitPrefab, spawnPoint, Quaternion.identity);
        unit.playerID = playerId;
        SetRandomParameters(unit);
        unit.gameObject.SetActive(true);
        return unit;
    }

    public PlayerPurchases CreatePlayerPurchases()
    {
        PlayerPurchases playerPurchases = Instantiate(playerPurchasesPrefab);
        playerPurchases.player = player;
        return playerPurchases;

    }

    public TemporaryBuilding CreateTemporaryMainBuilding(NetworkInstanceId playerId, BuildingEnum buildingType)
    {
        TemporaryBuilding building;
        switch (buildingType)
        {
            case BuildingEnum.MainBuilding:
                building = Instantiate(mainBuildingPrefab);
                break;
            case BuildingEnum.Library:
                building = Instantiate(libraryPrefab);
                break;
            case BuildingEnum.Barracks:
                building = Instantiate(barracksPrefab);
                break;
            case BuildingEnum.Infirmary:
                building = Instantiate(infirmaryPrefab);
                break;
            case BuildingEnum.House:
                building = Instantiate(housePrefab);
                break;
            case BuildingEnum.Mill:
                building = Instantiate(millPrefab);
                break;
            default:
                building = null;
                break;
        }
        building.playerID = playerId;
        return building;
    }

    private void SetRandomParameters(Unit unit)
    {
        int health = rnd.Next(100);
        int gathering = rnd.Next(100);
        int intelligence = rnd.Next(100);
        int swordsmanship = rnd.Next(100);
        int healing = rnd.Next(100);
        int building = rnd.Next(100);
        int accuracy = rnd.Next(100);

        int ratio = (health + gathering + intelligence + swordsmanship + healing + building + accuracy);

        unit.MaxHealth = 100 + health * sumOfProperties / ratio + 1;
        unit.Health = unit.MaxHealth;
        unit.Gathering = gathering * sumOfProperties / ratio + 1;
        unit.Intelligence = intelligence * sumOfProperties / ratio + 1;
        unit.Swordsmanship = swordsmanship * sumOfProperties / ratio + 1;
        unit.Healing = healing * sumOfProperties / ratio + 1;
        unit.Building = building * sumOfProperties / ratio + 1;
        unit.Accuracy = accuracy * sumOfProperties / ratio + 1;
    }
}
