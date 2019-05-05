using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour {

    [SerializeField]
    private GameObject unitRowPrefab;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private GameObject unitsArea;

    private List<UnitRow> unitRows = new List<UnitRow>();

    public void Show(Building building, List<Unit> units, Action<Unit> action)
    {
        nameText.text = building.Name;
        units.ForEach(u =>
        {
            UnitRow unitRow = Instantiate(unitRowPrefab, unitsArea.transform).GetComponent<UnitRow>();
            unitRow.SetActions(building, u, action);
            unitRow.SetText(u.Name, u.GetObjectDescription());
            unitRows.Add(unitRow);
        });
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        unitRows.ForEach(ur => Destroy(ur.gameObject));
        unitRows.Clear();
    }
}
