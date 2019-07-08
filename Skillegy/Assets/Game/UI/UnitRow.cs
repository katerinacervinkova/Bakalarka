using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitRow : MonoBehaviour {

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

    public void Init(Building building, Unit unit, Action<Unit> action)
    {
        this.unit = unit;
        this.building = building;
        if (action == null)
            actionButton.gameObject.SetActive(false);
        else
            actionButton.onClick.AddListener(() => action.Invoke(unit));
        removeButton.onClick.AddListener(() => 
        {
            building.Exit(unit);
            building.OnUnitsChange();
        });
        UpdateDescription();
    }

    public void UpdateDescription()
    {
        unitNameText.text = building.UnitName(unit);
        unitText.text = building.UnitText(unit);
    }
}
