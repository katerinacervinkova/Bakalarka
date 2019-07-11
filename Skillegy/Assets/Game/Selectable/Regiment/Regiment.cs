using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Regiment : Selectable {

    protected List<Unit> units;
    private int unitsToArrive = 0;
    private Vector3 destination = Vector3.positiveInfinity;

    public override string Name => $"Units({units.Count})";

    public override string GetObjectDescription()
    {
        return $"Health: {(int)units.Sum(u => u.Health)}/{(int)units.Sum(u => u.MaxHealth)}\n" +
            $"Gathering: {(int)units.Sum(u => u.Gathering)}\n" +
            $"Intelligence: {(int)units.Sum(u => u.Intelligence)}\n" +
            $"Swordsmanship: {(int)units.Sum(u => u.Swordsmanship)}\n" +
            $"Healing: {(int)units.Sum(u => u.Healing)}\n" +
            $"Building: {(int)units.Sum(u => u.Building)}";
    }

    public void SetUnits(List<Unit> units)
    {
        this.units = units;
        units.ForEach(u => u.Reg = this);
    }

    public void Remove(Unit unit)
    {
        // do not wait for unit to reach the destination if it has left the regiment
        if (unit.movementController.IsMoving)
            MovementCompleted(unit);

        units.Remove(unit);
        unit.Reg = null;
    }

    protected override void Update()
    {
        base.Update();
        // destroy the object, if there are no units or it is not selected
        if (PlayerState.Get(playerId).SelectedObject != this || units.Count == 0)
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
        UIManager.Instance.ShowPurchaseButtons(units[0].Purchases, this);
    }

    protected override void HideAllButtons()
    {
        base.HideAllButtons();
        if (owner.IsHuman)
            UIManager.Instance.HideTarget();
    }

    public override void SetGoal(Selectable goal) => units.ForEach(u => u.SetGoal(goal));


    /// <summary>
    /// Sets the destinations for the units randomly in a circle of radius depending on units' count
    /// </summary>
    /// <param name="hitPoint">desired destination</param>
    public override void SetGoal(Vector3 hitPoint)
    {
        unitsToArrive = units.Count;
        destination = hitPoint;
        if (owner.IsHuman && UIManager.Instance != null && PlayerState.Get(playerId).SelectedObject == this)
            UIManager.Instance.ShowTarget(destination);
        float ratio = Mathf.Sqrt(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            var v = Random.insideUnitCircle * ratio;
            units[i].SetJob(new JobGo(new Vector3(hitPoint.x + v.x, 0, hitPoint.z + v.y)));
        }
    }

    /// <summary>
    /// Hides the target UI when all units have arrived.
    /// </summary>
    /// <param name="unit">unit that has just arrived</param>
    public void MovementCompleted(Unit unit)
    {
        if (!float.IsPositiveInfinity(destination.x))
            unitsToArrive--;
        if (unitsToArrive == 0 && owner.IsHuman)
            UIManager.Instance.HideTarget();
    }

    public override Job GetOwnJob(Unit worker = null) => null;
}
