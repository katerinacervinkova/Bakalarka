using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitRow : MonoBehaviour {

    [SerializeField]
    private Text actionLabel;
    [SerializeField]
    private Button actionButton;
    [SerializeField]
    private Button removeButton;
    [SerializeField]
    private Text unitNameText;
    [SerializeField]
    private Text unitText;

    private Unit unit;
    private Building building;

    /// <summary>
    /// Initializes all variables and buttons.
    /// </summary>
    /// <param name="action">action to perform after clicking the action button. If null, action button is deactivated.</param>
    public void Init(Building building, Unit unit, Action<Unit> action, string actionName)
    {
        this.unit = unit;
        this.building = building;

        if (action == null)
            actionButton.gameObject.SetActive(false);
        else
        {
            actionButton.onClick.AddListener(() => action.Invoke(unit));
            if (actionName != null)
                actionLabel.text = actionName;
        }

        // removes the unit and updates the building window
        removeButton.onClick.AddListener(() => 
        {
            building.Exit(unit);
            building.OnUnitsChange();
        });

        UpdateDescription();
    }

    /// <summary>
    /// Shows unit's name and description relevant for this building.
    /// </summary>
    public void UpdateDescription()
    {
        unitNameText.text = building.UnitName(unit);
        unitText.text = building.UnitText(unit);
    }
}
