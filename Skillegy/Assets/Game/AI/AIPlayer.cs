using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    private System.Random rnd = new System.Random();

    public int playerId;
    public Player player;
    public PlayerState playerState;
    public GameState gameState;

    public List<Unit> SenseIdleUnits() => playerState.IdleUnits();

    public Unit SenseBestUnit(SkillEnum attribute) => playerState.BestUnit(attribute);
    public Unit SenseBestIdleUnit(SkillEnum attribute) => playerState.BestIdleUnit(attribute);

    public List<Unit> SenseGoodUnits(SkillEnum attribute, float bar) => playerState.GoodUnits(attribute, bar);
    public List<Unit> SenseGoodIdleUnits(SkillEnum attribute, float bar) => playerState.GoodIdleUnits(attribute, bar);

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

    public bool BuildBuilding(PurchasesEnum type)
    {
        var purchase = playerState.playerPurchases.Get(type);
        return purchase.IsActive(null) && purchase.Do(null);
    }
    public bool BuildBuilding(BuildingEnum type)
    {
        var purchase = playerState.playerPurchases.Get(type);
        return purchase.IsActive(null) && purchase.Do(null);
    }

    public bool PlaceBuilding()
    {
        var randomPosition = gameState.GetRandomDestination(player.transform.position, (SenseOwnBuildings().Count + 1) * 10);
        if (!float.IsPositiveInfinity(randomPosition.x))
            return PlaceBuilding(randomPosition);
        return false;
    }
    public bool PlaceBuilding(Vector3 position)
    {
        Unit unit = SenseClosestIdleUnit(position);
        if (unit == null)
            unit = SenseClosestUnit(position);
        if (unit == null)
            return false;
        return PlaceBuilding(position, unit);
    }
    public bool PlaceBuilding(Vector3 position, Unit unit)
    {
        if (playerState.BuildingToBuild == null)
            return false;
        if (SenseOwnUnits().Where(u => Vector3.Distance(position, u.transform.position) < 3).Any())
            return false;
        if (SenseVisibleEnemyUnits().Where(u => Vector3.Distance(position, u.transform.position) < 3).Any())
            return false;
        if (!playerState.MoveBuildingToBuild(position))
            return false;
        SelectObject(unit);
        playerState.PlaceBuilding();
        return true;
    }

    public bool BuildUnit(MainBuilding building) => playerState.playerPurchases.Get(PurchasesEnum.Unit).Do(building);
    public bool BuildUnit()
    {
        MainBuilding building = (MainBuilding)SenseOwnBuildings().Where(b => b is MainBuilding).FirstOrDefault();
        if (building == null)
            return false;
        return BuildUnit(building);
    }

    public bool GatherFromResource(Unit unit, Resource resource)
    {
        unit.SetGoal(resource);
        return true;
    }
    public bool GatherFood(Unit unit)
    {
        Resource res = SenseClosestVisibleResource<FoodResource>(unit.transform.position);
        if (res != null)
            return GatherFromResource(unit, res);
        Building building = RandomElement(SenseOwnBuildings().Where(b => b is Mill));
        if (building == null)
            return false;
        return EnterBuilding(unit, building);
    }
    public bool GatherWood(Unit unit)
    {
        Resource res = SenseClosestVisibleResource<WoodResource>(unit.transform.position);
        if (res != null)
            return GatherFromResource(unit, res);
        Building building = RandomElement(SenseOwnBuildings().Where(b => b is Sawmill));
        if (building == null)
            return false;
        return EnterBuilding(unit, building);
    }
    public bool GatherGold(Unit unit)
    {
        Resource res = SenseClosestVisibleResource<GoldResource>(unit.transform.position);
        if (res != null)
            return GatherFromResource(unit, res);
        Building building = RandomElement(SenseOwnBuildings().Where(b => b is Bank));
        if (building == null)
            return false;
        return EnterBuilding(unit, building);
    }

    public void Explore(Unit unit) => unit.SetJob(new JobExplore());
    public bool Explore()
    {
        var unit = RandomElement(SenseIdleUnits());
        if (unit == null)
            return false;
        Explore(unit);
        return true;
    }

    public bool EnterBuilding<T>(Unit unit = null, T building = null) where T : Building
    {
        if (building == null)
        {
            building = (T)RandomElement(SenseOwnBuildings().Where(b => b is T));
            if (building == null)
                return false;
        }
        if (unit == null)
        {
            unit = RandomElement(SenseIdleUnits());
            if (unit == null)
                return false;
        }
        if (building.UnitCount + 1 > building.UnitCapacity)
            return false;
        unit.SetGoal(building);
        return true;
    }
    public bool ExitBuilding(Building building, Unit unit = null)
    {
        if (unit == null)
            building.Exit();
        else
            building.Exit(unit);
        return true;
    }

    public bool TrainUnit(SkillEnum type, Unit unit = null)
    {
        if (unit == null)
        {
            unit = RandomElement(SenseIdleUnits());
            if (unit == null)
                return false;
        }
        switch (type)
        {
            case SkillEnum.Gathering:
                return GatherFood(unit) || GatherWood(unit) || GatherGold(unit);
            case SkillEnum.Intelligence:
                var building = RandomElement(SenseOwnBuildings().Where(b => b is Library && (b as Library).Focus == Library.FocusEnum.Intelligence));
                if (building == null)
                    return false;
                return EnterBuilding(unit, building);
            case SkillEnum.Swordsmanship:
                building = RandomElement(SenseOwnBuildings().Where(b => b is Barracks));
                if (building == null)
                    return false;
                return EnterBuilding(unit, building);
            case SkillEnum.Healing:
                building = RandomElement(SenseOwnBuildings().Where(b => b is Library && (b as Library).Focus == Library.FocusEnum.Healing));
                if (building != null)
                    return EnterBuilding(unit, building);
                building = RandomElement(SenseOwnBuildings().Where(b => b is Infirmary));
                if (building == null)
                    return false;
                return EnterBuilding(unit, building);
            case SkillEnum.Building:
                var tempBuilding = SenseClosestTemporaryBuilding(unit.transform.position);
                if (tempBuilding != null)
                {
                    unit.SetGoal(tempBuilding);
                    return true;
                }
                building = RandomElement(SenseOwnBuildings().Where(b => b is Library && (b as Library).Focus == Library.FocusEnum.Building));
                if (building == null)
                    return false;
                return EnterBuilding(unit, building);
            default:
                return false;
        }
    }

    public bool Attack(Unit unit = null, Selectable target = null)
    {
        if (unit == null)
            unit = RandomElement(SenseIdleUnits());
        if (unit == null)
            return false;
        if (target == null)
            target = RandomElement(SenseVisibleEnemyUnits());
        if (target == null)
            target = RandomElement(SenseVisibleEnemyBuildings());
        if (target == null)
            target = RandomElement(SenseVisibleEnemyTemporaryBuildings());
        if (target == null)
            return false;
        unit.SetGoal(target);
        return true;
    }
    
    public void GetPurchaseCost(PurchasesEnum purchaseEnum, out int food, out int wood, out int gold, out int population)
    {
        var purchase = playerState.playerPurchases.Get(purchaseEnum);
        food = purchase.food;
        wood = purchase.wood;
        gold = purchase.gold;
        population = purchase.population;
    }
    public void CheckPurchaseCost(int food, int wood, int gold)
    {
        if (playerState.Food < food)
        {
            Unit unit = SenseBestIdleUnit(SkillEnum.Gathering);
            if (unit != null)
                GatherFood(unit);
        }
        if (playerState.Wood < wood)
        {
            Unit unit = SenseBestIdleUnit(SkillEnum.Gathering);
            if (unit != null)
                GatherWood(unit);
        }
        if (playerState.Gold < gold)
        {
            Unit unit = SenseBestIdleUnit(SkillEnum.Gathering);
            if (unit != null)
                GatherGold(unit);
        }
    }
    public bool DoPurchase(PurchasesEnum purchasesEnum)
    {
        Selectable selectable;
        switch (purchasesEnum)
        {
            case PurchasesEnum.Barracks:
            case PurchasesEnum.House:
            case PurchasesEnum.Infirmary:
            case PurchasesEnum.Library:
            case PurchasesEnum.MainBuilding:
            case PurchasesEnum.Mill:
            case PurchasesEnum.Sawmill:
            case PurchasesEnum.Bank:
                return BuildBuilding(purchasesEnum);
            case PurchasesEnum.Unit:
            case PurchasesEnum.StoneAge:
            case PurchasesEnum.IronAge:
            case PurchasesEnum.DiamondAge:
                selectable = RandomElement(SenseOwnBuildings().Where(b => b is MainBuilding));
                break;
            case PurchasesEnum.Books1:
            case PurchasesEnum.Books2:
            case PurchasesEnum.Books3:
            case PurchasesEnum.Books4:
            case PurchasesEnum.Books5:
            case PurchasesEnum.BuildingBooks:
            case PurchasesEnum.MedicineBooks1:
            case PurchasesEnum.MedicineBooks2:
            case PurchasesEnum.Intelligence:
            case PurchasesEnum.Building:
            case PurchasesEnum.Healing:
                selectable = RandomElement(SenseOwnBuildings().Where(b => b is Library));
                break;
            case PurchasesEnum.Gear1:
            case PurchasesEnum.Gear2:
            case PurchasesEnum.Gear3:
            case PurchasesEnum.Gear4:
            case PurchasesEnum.Gear5:
                selectable = RandomElement(SenseOwnBuildings().Where(b => b is Barracks));
                break;
            case PurchasesEnum.Flour:
            case PurchasesEnum.Bread:
                selectable = RandomElement(SenseOwnBuildings().Where(b => b is Mill));
                break;
            default:
                selectable = null;
                break;
        }
        if (selectable == null)
            return false;
        Purchase purchase = playerState.playerPurchases.Get(purchasesEnum);
        if (!purchase.IsActive(selectable))
            return false;
        return purchase.Do(selectable);
    }

    private T RandomElement<T>(IEnumerable<T> collection) => collection.Take(rnd.Next(collection.Count()) + 1).LastOrDefault();
    private T RandomElement<T>(List<T> collection)
    {
        if (collection.Count > 0)
            return collection[rnd.Next(collection.Count())];
        return default(T);
    }
}
