using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Regiment : Selectable {
    protected enum RState { Standing, Assembling, Moving }

    protected RState state;

    protected Vector3 destination;
    protected List<Unit> units;
    
    protected override void Awake()
    {
        buttons = new List<Button>();
    }

    protected override void Start()
    {
        
    }

    protected override void Update()
    {
        if (units.Count == 0)
            Destroy(gameObject);
        else if (units.TrueForAll(u => u.IsStanding))
            if (state == RState.Assembling)
                CoordinatedMovement();
            else if (state == RState.Moving)
                if (!selected)
                    Destroy(gameObject);
                else
                    state = RState.Standing;
    }

    public override void SetSelection(bool selected, Player player)
    {
        foreach (var unit in units)
            unit.SetSelection(selected, player);
        if (!selected && state == RState.Standing)
            Destroy(gameObject);
    }

    public override void RightMouseClickGround(GameObject hitObject, Vector3 hitPoint)
    {
        destination = hitPoint;
        units.ForEach(
            u =>
            {
                if (u.regiment != this && u.regiment != null)
                    u.regiment.Remove(u);
                u.regiment = this;
            });
        Vector3 mean = new Vector3(units.Sum(u => u.transform.position.x), units.Sum(u => u.transform.position.y), units.Sum(u => u.transform.position.z)) / units.Count;
        mean += (hitPoint - mean).normalized * units.Max(u => Vector3.Distance(u.transform.position, mean));
        SetDestination(mean);
        state = RState.Assembling;
    }

    public void Remove(Unit unit)
    {
        units.Remove(unit);
    }
    private void CoordinatedMovement()
    {
        SetDestination(destination);
        state = RState.Moving;
    }

    private void SetDestination(Vector3 destination)
    {
        gridGraph.SetDestination(destination, units);
    }

    public override void DrawBottomBar()
    {
        if (units.Count == 0)
            return;
        units[0].DrawBottomBar();
    }
    public void SetUnits(List<Unit> units)
    {
        this.units = units;
    }
}
