using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour {

    public Player player;
    public Transform MainBuildingPrefab;

    // unit buttons 
    public Button CreateMainBuildingButton;

    // main building buttons
    public Button CreateUnitButton;

    // Use this for initialization
    void Start () {
    }

    public void SetActive(Unit unit, bool active)
    {
        if (active)
            CreateMainBuildingButton.onClick.AddListener(() => player.SetWorkerAndBuilding(player.factory.CreateTemporaryMainBuilding(), unit));
        else
            CreateMainBuildingButton.onClick.RemoveAllListeners();
        CreateMainBuildingButton.gameObject.SetActive(active);
    }
    public void SetActive(Building building, bool active)
    {
        if (active)
            CreateUnitButton.onClick.AddListener(building.CreateUnit);
        else
            CreateUnitButton.onClick.RemoveAllListeners();
        CreateUnitButton.gameObject.SetActive(active);
    }

    private Transform TempMainBuilding()
    {
        return Instantiate(MainBuildingPrefab);
    }
}