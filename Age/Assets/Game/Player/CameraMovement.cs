using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public InputOptions inputOptions;
    public GameWindow gameWindow;

    readonly float panSpeed = 20;
    readonly int panBorderThickness = 10;

    readonly Vector3 mainCameraMin = new Vector3(-200, 0, -200);
    readonly Vector3 mainCameraMax = new Vector3(160, 0, 160);


    void Update ()
    {
        if (inputOptions.MoveCameraEnabled)
        {
            MoveCamera();
            ZoomCamera();
        }
    }

    private void ZoomCamera()
    {
        Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 200;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 25, 70);
    }

    private void MoveCamera()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey("w") || Input.GetKey("d"))
            movement.x += panSpeed * Time.deltaTime;
        if (Input.GetKey("d") || Input.GetKey("s"))
            movement.z -= panSpeed * Time.deltaTime;
        if (Input.GetKey("s") || Input.GetKey("a"))
            movement.x -= panSpeed * Time.deltaTime;
        if (Input.GetKey("a") || Input.GetKey("w"))
            movement.z += panSpeed * Time.deltaTime;

        float m = HorizontalMovement(Input.mousePosition.x);
        movement += new Vector3(m, 0, -m);
        m = VerticalMovement(Input.mousePosition.y);
        movement += new Vector3(m, 0, m);

        if (movement == Vector3.zero)
            return;

        Vector3 pos = Camera.main.transform.position + movement;
        pos.x = Mathf.Clamp(pos.x, mainCameraMin.x, mainCameraMax.x);
        pos.z = Mathf.Clamp(pos.z, mainCameraMin.z, mainCameraMax.z);
        transform.position = pos;
    }

    private float HorizontalMovement(float pos)
    {
        if (Math.Abs(pos - gameWindow.LeftBorder) <= panBorderThickness)
            return -panSpeed * Time.deltaTime;
        if (Math.Abs(pos - gameWindow.RightBorder) <= panBorderThickness)
            return panSpeed * Time.deltaTime;
        return 0;
    }

    private float VerticalMovement(float pos)
    {
        if (Math.Abs(pos - gameWindow.BottomBorder) <= panBorderThickness)
            return -panSpeed * Time.deltaTime;
        if (Math.Abs(pos - gameWindow.TopBorder) <= panBorderThickness)
            return panSpeed * Time.deltaTime;
        return 0;
    }
}
