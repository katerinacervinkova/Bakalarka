using UnityEngine;

public class PlayerInputOptions : MonoBehaviour {

    public bool MoveCameraEnabled { get; set; }

    private void Start()
    {
        MoveCameraEnabled = true;
    }
}