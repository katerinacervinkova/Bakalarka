using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public int playerId;
    public Player player;
    public PlayerState playerState;
    public GameState gameState;

    public List<Unit> SenseIdleUnits() => playerState.IdleUnits();

    public Unit SenseBestUnit(AttEnum attribute) => playerState.BestUnit(attribute);
    public Unit SenseBestIdleUnit(AttEnum attribute) => playerState.BestIdleUnit(attribute);

    public List<Unit> SenseGoodUnits(AttEnum attribute, float bar) => playerState.GoodUnits(attribute, bar);
    public List<Unit> SenseGoodIdleUnits(AttEnum attribute, float bar) => playerState.GoodIdleUnits(attribute, bar);

    public List<Unit> SenseOwnUnits() => playerState.units;
    public List<Building> SenseOwnBuildings() => playerState.buildings;
    public List<TemporaryBuilding> SenseOwnTemporaryBuildings() => playerState.temporaryBuildings;

    public Unit SenseClosestUnit(Vector3 destination) => SenseOwnUnits() .OrderBy(u => Vector3.Distance(u.transform.position, destination)).FirstOrDefault();
    public Unit SenseClosestIdleUnit(Vector3 destination) => SenseIdleUnits().OrderBy(u => Vector3.Distance(u.transform.position, destination)).FirstOrDefault();
    public Building SenseClosestBuilding(Vector3 destination) => SenseOwnBuildings().OrderBy(u => Vector3.Distance(u.transform.position, destination)).FirstOrDefault();
    public TemporaryBuilding SenseClosestTemporaryBuilding(Vector3 destination) => SenseOwnTemporaryBuildings().OrderBy(u => Vector3.Distance(u.transform.position, destination)).FirstOrDefault();
    public T SenseClosestVisibleResource<T>(Vector3 destination) where T : Resource => gameState.ClosestVisibleResource<T>(destination, playerId);

    public List<Unit> SenseVisibleEnemyUnits() => gameState.VisibleEnemyUnits(playerId);
    public List<Building> SenseVisibleEnemyBuildings() => gameState.VisibleEnemyBuildings(playerId);
    public List<TemporaryBuilding> SenseVisibleEnemyTemporaryBuildings() => gameState.VisibleEnemyTemporaryBuildings(playerId);
    public List<Resource> SenseVisibleResources() => gameState.VisibleResources(playerId);

    public void SelectObject(Selectable selectable) => playerState.Select(selectable);
    public void DeselectObject() => playerState.Deselect();

    public bool BuildBuilding(BuildingEnum type, Vector3 position)
    {
        Unit unit = SenseClosestIdleUnit(position);
        if (unit == null)
            unit = SenseClosestUnit(position);
        if (unit != null)
            return BuildBuilding(type, position, unit);
        return false;
    }

    public bool BuildBuilding(BuildingEnum type, Vector3 position, Commandable commandable)
    {
        SelectObject(commandable);
        if (playerState.playerPurchases.Get(type).Do(commandable))
        {
            var pos = GameState.Instance.GetClosestFreePosition(position, playerId);
            if (!float.IsPositiveInfinity(pos.x))
            {
                StartCoroutine(PlaceBuilding(pos));
                return true;
            }
        }
        return false;
    }

    private IEnumerator PlaceBuilding(Vector3 position)
    {
        while(playerState.BuildingToBuild == null)
            yield return new WaitForSeconds(.1f);
        playerState.MoveBuildingToBuild(position);
        playerState.PlaceBuilding();
    }

    public bool BuildUnit(MainBuilding building) => playerState.playerPurchases.Get(PurchasesEnum.Unit).Do(building);
    public bool BuildUnit()
    {
        MainBuilding building = (MainBuilding)SenseOwnBuildings().Where(b => b is MainBuilding).FirstOrDefault();
        if (building == null)
            return false;
        return BuildUnit(building);
    }

    public void GatherFromResource(Commandable commandable, Resource resource) => commandable.SetGoal(resource);
    public void Gather<T>(Commandable commandable) where T : Resource
    {
        Resource res = SenseClosestVisibleResource<T>(commandable.transform.position);
        if (res != null)
            GatherFromResource(commandable, res);
    }

    public void Explore(Unit unit) => unit.SetJob(new JobExplore());
}
