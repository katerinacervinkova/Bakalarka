using System;
using UnityEngine;

/// <summary>
/// Class that handles camera movement and zooming
/// </summary>
public class CameraMovement : MonoBehaviour {

    [SerializeField]
    private InputOptions inputOptions;
    [SerializeField]
    private GameWindow gameWindow;

    private readonly float panSpeed = 20;
    private readonly int panBorderThickness = 10;

    private readonly Vector3 panLimit = new Vector3(200, 0, 200);

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

        if (!inputOptions.MouseOverUI)
        {
            float m = HorizontalMovement(Input.mousePosition.x);
            movement += new Vector3(m, 0, -m);
            m = VerticalMovement(Input.mousePosition.y);
            movement += new Vector3(m, 0, m);
        }

        if (movement == Vector3.zero)
            return;

        Vector3 pos = transform.position + movement;
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.z, panLimit.z);
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

    public void MinimapMove(Vector3 position)
    {
        transform.position = position;
    }
}
