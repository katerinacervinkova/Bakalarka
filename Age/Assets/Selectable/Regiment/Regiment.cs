using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Age;

public class Regiment : Selectable {

    public List<Unit> units;
    
    protected override void Start()
    {
        
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

    public override void RightMouseClick(GameObject hitObject, Vector3 hitPoint)
    {
        foreach (var unit in units)
            unit.RightMouseClick(hitObject, hitPoint);
    }
}
