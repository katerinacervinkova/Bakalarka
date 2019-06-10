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
    public List<Building> SenseOwnBuilding() => playerState.buildings;
    public List<TemporaryBuilding> SenseOwnTemporaryBuilding() => playerState.temporaryBuildings;

    public Unit SenseClosestUnit(Vector3 destination) => playerState.units.OrderBy(u => Vector3.Distance(u.transform.position, destination)).FirstOrDefault();
    public Building SenseClosestBuilding(Vector3 destination) => playerState.buildings.OrderBy(u => Vector3.Distance(u.transform.position, destination)).FirstOrDefault();
    public TemporaryBuilding SenseClosestTemporaryBuilding(Vector3 destination) => playerState.temporaryBuildings.OrderBy(u => Vector3.Distance(u.transform.position, destination)).FirstOrDefault();
    public T SenseClosestVisibleResource<T>(Vector3 destination) where T : Resource => gameState.ClosestVisibleResource<T>(destination, playerId);

    public List<Unit> SenseVisibleEnemyUnits() => gameState.VisibleEnemyUnits(playerId);
    public List<Building> SenseVisibleEnemyBuildings() => gameState.VisibleEnemyBuildings(playerId);
    public List<TemporaryBuilding> SenseVisibleEnemyTemporaryBuildings() => gameState.VisibleEnemyTemporaryBuildings(playerId);
    public List<Resource> SenseVisibleResources() => gameState.VisibleResources(playerId);
}
