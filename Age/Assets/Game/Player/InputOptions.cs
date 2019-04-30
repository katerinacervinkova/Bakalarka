using UnityEngine;
using UnityEngine.EventSystems;

public class InputOptions : MonoBehaviour {

    public bool MoveCameraEnabled { get; set; }
    private int fingerID = -1;

    private void Awake()
    {
        #if !UNITY_EDITOR
            fingerID = 0; 
        #endif
        MoveCameraEnabled = true;

    }

    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject(fingerID);
    }

    private void OnApplicationFocus(bool pauseStatus)
    {
        MoveCameraEnabled = pauseStatus;
    }
}