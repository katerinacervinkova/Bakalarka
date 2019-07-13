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

    // Rows with units' information
    private List<UnitRow> unitRows = new List<UnitRow>();

    /// <summary>
    /// Shows the window and creates a new row for every unit in the building.
    /// </summary>
    public void Show(Building building, List<Unit> units, Action<Unit> action)
    {
        nameText.text = building.Name;
        units.ForEach(u =>
        {
            UnitRow unitRow = Instantiate(unitRowPrefab, unitsArea.transform).GetComponent<UnitRow>();
            unitRow.Init(building, u, action);
            unitRows.Add(unitRow);
        });
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the window and destroys all units' rows
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        unitRows.ForEach(ur => Destroy(ur.gameObject));
        unitRows.Clear();
    }

    /// <summary>
    /// Updates information about all units.
    /// </summary>
    public void UpdateDescriptions()
    {
        unitRows.ForEach(ur => ur.UpdateDescription());
    }
}
