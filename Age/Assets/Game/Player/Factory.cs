using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Factory : MonoBehaviour
{
    protected System.Random rnd = new System.Random();
    protected int sumOfProperties = 35;

    public Player player;
    [SerializeField]
    private TemporaryBuilding mainBuildingPrefab;
    [SerializeField]
    private TemporaryBuilding libraryPrefab;
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
        int agility = rnd.Next(100);
        int healing = rnd.Next(100);
        int building = rnd.Next(100);
        int accuracy = rnd.Next(100);

        int ratio = (health + gathering + intelligence + agility + healing + building + accuracy);

        unit.MaxHealth = 100 + health * sumOfProperties / ratio + 1;
        unit.Health = unit.MaxHealth;
        unit.Gathering = gathering * sumOfProperties / ratio + 1;
        unit.Intelligence = intelligence * sumOfProperties / ratio + 1;
        unit.Agility = agility * sumOfProperties / ratio + 1;
        unit.Healing = healing * sumOfProperties / ratio + 1;
        unit.Building = building * sumOfProperties / ratio + 1;
        unit.Accuracy = accuracy * sumOfProperties / ratio + 1;
    }
}
