using UnityEngine;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour {

    public Transform MainBuildingPrefab;

    // unit buttons 
    public Button CreateMainBuildingButton;

    // main building buttons
    public Button CreateUnitButton;


    public void SetActive(GameState gameState, Selectable selectable, bool active)
    {
        if (active)
        {
            if (selectable is Resource || selectable is TemporaryBuilding)
                return;
            if (selectable is Building)
            {
                CreateUnitButton.onClick.AddListener(() => gameState.CreateUnit(selectable as Building));
                CreateUnitButton.gameObject.SetActive(true);
            }
            else if (selectable is Unit)
            {
                CreateMainBuildingButton.onClick.AddListener(() => gameState.CreateTemporaryMainBuilding(selectable as Unit));
                CreateMainBuildingButton.gameObject.SetActive(true);
            }
            else if (selectable is Regiment)
            {
                CreateMainBuildingButton.onClick.AddListener(() => gameState.CreateTemporaryMainBuilding(((Regiment)selectable).GetFirstUnit()));
                CreateMainBuildingButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (CreateMainBuildingButton != null)
            {
                CreateMainBuildingButton.onClick.RemoveAllListeners();
                CreateMainBuildingButton.gameObject.SetActive(false);
            }
            if (CreateUnitButton != null)
            {
                CreateUnitButton.onClick.RemoveAllListeners();
                CreateUnitButton.gameObject.SetActive(false);
            }
        }
    }

    private Transform TempMainBuilding()
    {
        return Instantiate(MainBuildingPrefab);
    }


}