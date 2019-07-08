using UnityEngine;
using UnityEngine.EventSystems;

public class InputOptions : MonoBehaviour {

    public bool MoveCameraEnabled { get; set; }

    public bool MouseOverUI => EventSystem.current.IsPointerOverGameObject();

    private void Awake()
    {
        MoveCameraEnabled = true;
    }


    private void OnApplicationFocus(bool pauseStatus)
    {
        MoveCameraEnabled = pauseStatus;
    }
}