using UnityEngine;
using Age;

public class PlayerInputOptions : MonoBehaviour {

    public bool MoveCameraEnabled { get; set; }

    private void Start()
    {
        MoveCameraEnabled = true;
    }
}