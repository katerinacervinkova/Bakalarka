using UnityEngine;

public class InputOptions : MonoBehaviour {

    public bool MoveCameraEnabled { get; set; }

    private void Start()
    {
        MoveCameraEnabled = true;
    }

    private void OnApplicationFocus(bool pauseStatus)
    {
        MoveCameraEnabled = pauseStatus;
    }
}