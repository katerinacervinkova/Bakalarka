using System.Collections.Generic;
using UnityEngine;

public class MapSquare : MonoBehaviour {

    public int playerId;
    public Vector2 squareId;

    public bool activated = false;
    public bool wasActive = false;

    public bool uncovered = false;

    [SerializeField]
    private GameObject transparent;
    [SerializeField]
    private GameObject nontransparent;
    [SerializeField]
    private GameObject transparentMinimap;
    [SerializeField]
    private GameObject nontransparentMinimap;


    public List<MapSquare> AdjoiningSquares;

    public List<Unit> EnemyUnits { get; private set; } = new List<Unit>();
    public List<Unit> FriendlyUnits { get; private set; } = new List<Unit>();
    public List<Building> EnemyBuildings { get; private set; } = new List<Building>();
    public List<Building> FriendlyBuildings { get; private set; } = new List<Building>();
    public List<TemporaryBuilding> EnemyTemporaryBuildings { get; private set; } = new List<TemporaryBuilding>();
    public List<TemporaryBuilding> FriendlyTemporaryBuildings { get; private set; } = new List<TemporaryBuilding>();
    public List<Resource> Resources { get; private set; } = new List<Resource>();


    public bool ContainsFriend => FriendlyUnits.Count > 0 || FriendlyBuildings.Count > 0 || FriendlyTemporaryBuildings.Count > 0;


    /// <summary>
    /// Uncoveres the square if activated and covers it if not.
    /// </summary>
    public void UpdateVisibility()
    {
        if (activated)
            Uncover();
        else
            Cover();
        activated = false;
    }

    private void Cover()
    {
        SetVisibility(false);
        if (wasActive)
            SetActiveTransparent(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Uncover()
    {
        SetVisibility(true);
        if (!wasActive)
        {
            if (!uncovered)
            {
                Destroy(nontransparent);
                Destroy(nontransparentMinimap);
                uncovered = true;
            }
            SetActiveTransparent(false);
        }
    }

    /// <summary>
    /// Shows or hides the transparent part of the square.
    /// </summary>
    /// <param name="active"></param>
    private void SetActiveTransparent(bool active)
    {
        transparent.SetActive(active);
        transparentMinimap.SetActive(active);
        wasActive = !active;
    }

    /// <summary>
    /// Sets all enemy objects and resources (in)visible.
    /// </summary>
    private void SetVisibility(bool visible)
    {
        SetVisibility(visible, EnemyUnits);
        SetVisibility(visible, EnemyTemporaryBuildings);
        SetVisibility(visible, EnemyBuildings);
        SetVisibility(visible, Resources);
    }

    private void SetVisibility<T>(bool visible, List<T> selectables) where T : Selectable
    {
        for (int i = 0; i < selectables.Count; i++)
        {
            if (selectables[i] == null)
            {
                selectables.RemoveAt(i);
                i--;
                continue;
            }
            selectables[i].SetVisibility(visible);
        }
    }

    public void Add(Unit unit)
    {
        if (unit.hasAuthority && unit.playerId == playerId)
            FriendlyUnits.Add(unit);
        else
            EnemyUnits.Add(unit);
    }

    public void Add(Building building)
    {
        if (building.hasAuthority && building.playerId == playerId)
            FriendlyBuildings.Add(building);
        else
            EnemyBuildings.Add(building);
    }

    public void Add(TemporaryBuilding building)
    {
        if (building.hasAuthority && building.playerId == playerId)
            FriendlyTemporaryBuildings.Add(building);
        else
            EnemyTemporaryBuildings.Add(building);
    }

    public void Add(Resource resource) => Resources.Add(resource);

    public void Remove(Unit unit)
    {
        if (unit.hasAuthority && unit.playerId == playerId)
            FriendlyUnits.Remove(unit);
        else
            EnemyUnits.Remove(unit);
    }

    public void Remove(TemporaryBuilding building)
    {
        if (building.hasAuthority && building.playerId == playerId)
            FriendlyTemporaryBuildings.Remove(building);
        else
            EnemyTemporaryBuildings.Remove(building);
    }

    public void Remove(Building building)
    {
        if (building.hasAuthority && building.playerId == playerId)
            FriendlyBuildings.Remove(building);
        else
            EnemyBuildings.Remove(building);
    }

    public void Remove(Resource resource) => Resources.Remove(resource);
}