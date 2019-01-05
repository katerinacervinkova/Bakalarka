using UnityEngine;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour {

    public Player player;
    public Transform MainBuildingPrefab;

    // unit buttons 
    public Button CreateMainBuildingButton;

    // main building buttons
    public Button CreateUnitButton;


    public void SetActive(Commandable worker, bool active)
    {
        if (active)
            CreateMainBuildingButton.onClick.AddListener(() => player.SetWorkerAndBuilding(player.factory.CreateTemporaryMainBuilding(), worker));
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