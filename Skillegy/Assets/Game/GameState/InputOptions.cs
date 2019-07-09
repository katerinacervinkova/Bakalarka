using UnityEngine;
using UnityEngine.EventSystems;

public class InputOptions : MonoBehaviour {

    public bool MoveCameraEnabled { get; set; }

    // returns true if the mouse is currently over any UI element
    public bool MouseOverUI => EventSystem.current.IsPointerOverGameObject();

    private void Awake()
    {
        MoveCameraEnabled = true;
    }

    // disables moving camera when not focused on the application
    private void OnApplicationFocus(bool pauseStatus)
    {
        MoveCameraEnabled = pauseStatus;
    }
}