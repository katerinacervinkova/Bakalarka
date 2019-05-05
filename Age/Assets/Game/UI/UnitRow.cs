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

    public void SetActions(Building building, Unit unit, Action<Unit> action)
    {
        if (action == null)
            actionButton.gameObject.SetActive(false);
        else
            actionButton.onClick.AddListener(() => action.Invoke(unit));
        removeButton.onClick.AddListener(() => 
        {
            building.Exit(unit);
            building.OnUnitsChange();
        });
    }

    public void SetText(string name, string text)
    {
        unitText.text = text;
        unitNameText.text = name;
    }
}
