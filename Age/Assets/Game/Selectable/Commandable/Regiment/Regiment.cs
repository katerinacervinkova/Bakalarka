using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Regiment : Commandable {

    protected List<Unit> units;
    private int unitsToArrive = 0;

    public override string Name => $"Units({units.Count})";

    protected virtual void Update()
    {
        if (PlayerState.Instance.SelectedObject != this || units.Count == 0)
        {
            units.ForEach(u => u.Reg = null);
            Destroy(gameObject);
        }
    }

    public override void SetSelection(bool selected)
    {
        units.ForEach(u => u.SetVisualSelection(selected));
        if (UIManager.Instance != null)
        {
            if (selected)
                ShowAllButtons();
            else
                HideAllButtons();
        }
    }

    protected override void ShowAllButtons()
    {
        UIManager.Instance.ShowButtons(units[0].Purchases);
    }

    protected override void HideAllButtons()
    {
        base.HideAllButtons();
        HideTarget();
    }

    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        unitsToArrive = units.Count;
        destination = hitPoint;
        ShowTarget();
        float ratio = Mathf.Sqrt(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            var v = Random.insideUnitCircle * ratio;
            units[i].SetJob(new JobGo(new Vector3(hitPoint.x + v.x, 0, hitPoint.z + v.y)));
        }
    }

    public void Remove(Unit unit)
    {
        if (unit.IsMoving)
            MovementCompleted(unit);
        units.Remove(unit);
        unit.Reg = null;
    }

    public override string GetObjectDescription()
    {
        return $"Health: {(int)units.Sum(u => u.Health)}/{(int)units.Sum(u => u.MaxHealth)}\n" +
            $"Strength: {(int)units.Sum(u => u.Gathering)}\n" +
            $"Intelligence: {(int)units.Sum(u => u.Intelligence)}\n" +
            $"Swordsmanship: {(int)units.Sum(u => u.Swordsmanship)}\n" +
            $"Healing: {(int)units.Sum(u => u.Healing)}\n" +
            $"Building: {(int)units.Sum(u => u.Building)}\n" +
            $"Accuracy: {(int)units.Sum(u => u.Accuracy)}";
    }
    public void MovementCompleted(Unit unit)
    {
        if (IsMoving)
            unitsToArrive--;
        if (unitsToArrive == 0)
            HideTarget();
    }

    public void SetUnits(List<Unit> units)
    {
        this.units = units;
        units.ForEach(u => u.Reg = this);
    }

    public override void SetGoal(Selectable goal)
    {
        units.ForEach(u => u.SetGoal(goal));
    }

    protected override void InitPurchases() { }
}
