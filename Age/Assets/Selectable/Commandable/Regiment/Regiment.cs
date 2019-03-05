using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Regiment : Commandable {
    protected enum RState { Standing, Assembling, Moving }

    protected RState state;

    protected Vector3 destination;
    protected List<Unit> units;

    public override bool Arrived { get { return units.TrueForAll(u => u.Arrived); } }

    protected override void Update()
    {
        if (units.Count == 0)
            Destroy(gameObject);
        else if (units.TrueForAll(u => u.Arrived))
            if (state == RState.Assembling)
                CoordinatedMovement();
            else if (state == RState.Moving)
                if (!Selected)
                    Destroy(gameObject);
                else
                    state = RState.Standing;
    }

    public override void SetSelection(bool selected, Player player, BottomBar bottomBar)
    {
        if (!selected && state == RState.Standing)
            Destroy(gameObject);
        else
            foreach(Unit unit in units)
                SetSelection(selected, player, bottomBar);
    }

    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        destination = hitPoint;
        units.ForEach(
            u =>
            {
                if (u.Reg != this && u.Reg != null)
                    u.Reg.Remove(u);
                u.Reg = this;
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

    public override void SetDestination(Vector3 destination)
    {
        //gridGraph.SetDestination(destination, units);
    }
    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        if (units.Count == 0)
            return;
        nameText.text = Name;
        selectedObjectText.text = string.Format("Health: {0}/{1}", units.Sum(u => u.Health), units.Sum(u => u.MaxHealth))
        + "\nStrength: " + units.Sum(u => u.Strength) + "\nIntelligence: " + units.Sum(u => u.Intelligence)
        + "\nAgility: " + units.Sum(u => u.Agility) + "\nHealing: " + units.Sum(u => u.Healing)
        + "\nCrafting: " + units.Sum(u => u.Crafting) + "\nAccuracy: " + units.Sum(u => u.Accuracy);
    }
    public void SetUnits(List<Unit> units)
    {
        this.units = units;
    }

    public override void DrawHealthBar()
    {
        units.ForEach(u => u.DrawHealthBar());
    }

    public override void SetGoal(Selectable goal)
    {
        Job following = goal.CreateJob(this);
        units.ForEach(u => u.SetJob(new JobGo(u, goal.transform.position, following)));
    }
}
