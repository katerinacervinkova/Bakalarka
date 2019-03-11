using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Regiment : Commandable {

    protected Vector3 destination;
    protected List<Unit> units;

    protected override void Update()
    {
        if (!Selected || units.Count == 0)
        {
            Destroy(gameObject);
            units.ForEach(u => u.Reg = null);
        }
    }

    public override void SetSelection(bool selected, Player player, BottomBar bottomBar)
    {
        Selected = selected;
        foreach (Unit unit in units)
            unit.SetSelection(selected, player);
        bottomBar.SetActive(player, units[0].Transactions, selected);
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public Unit GetFirstUnit()
    {
        return units[0];
    }
    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        units.ForEach(u => u.SetJob(new JobGo(u, hitPoint)));
    }

    public void Remove(Unit unit)
    {
        units.Remove(unit);
        unit.Reg = null;
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
        units.ForEach(u => u.Reg = this);
    }

    public override void DrawHealthBar()
    {
        units.ForEach(u => u.DrawHealthBar());
    }

    public override void SetGoal(Selectable goal)
    {
        units.ForEach(u => u.SetGoal(goal));
    }
}
