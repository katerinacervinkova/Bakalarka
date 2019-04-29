using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public override void SetSelection(bool selected, Player player)
    {
        Selected = selected;
        foreach (Unit unit in units)
            unit.SetSelection(selected, player);
        if (selected)
            UIManager.Instance.ShowButtons(units[0].Purchases);
        else
            UIManager.Instance.HideButtons();

    }

    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        float ratio = Mathf.Sqrt(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            var v = Random.insideUnitCircle * ratio;
            units[i].SetJob(new JobGo(new Vector3(hitPoint.x + v.x, 0, hitPoint.z + v.y)));
        }
    }

    public void Remove(Unit unit)
    {
        units.Remove(unit);
        unit.Reg = null;
    }

    public override string GetObjectDescription()
    {
        return $"Health: {(int)units.Sum(u => u.Health)}/{(int)units.Sum(u => u.MaxHealth)}\n" +
            $"Strength: {(int)units.Sum(u => u.Gathering)}\n" +
            $"Intelligence: {(int)units.Sum(u => u.Intelligence)}\n" +
            $"Agility: {(int)units.Sum(u => u.Agility)}\n" +
            $"Healing: {(int)units.Sum(u => u.Healing)}\n" +
            $"Building: {(int)units.Sum(u => u.Building)}\n" +
            $"Accuracy: {(int)units.Sum(u => u.Accuracy)}";
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
