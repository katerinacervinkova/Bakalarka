using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Regiment : Selectable {

    public List<Unit> units;
    
    protected override void Awake()
    {
        buttons = new List<Button>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetSelection(bool selected, Player player)
    {
        foreach (var unit in units)
            unit.SetSelection(selected, player);
        if (!selected)
            Destroy(gameObject);
    }

    public override void RightMouseClickGround(GameObject hitObject, Vector3 hitPoint)
    {
        foreach (var unit in units)
            unit.RightMouseClickGround(hitObject, hitPoint);
    }

    public override void DrawBottomBar()
    {
        if (units.Count == 0)
            return;
        units[0].DrawBottomBar();
    }
}
