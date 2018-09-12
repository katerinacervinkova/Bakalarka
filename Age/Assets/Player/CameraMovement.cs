using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Age;

public class CameraMovement : MonoBehaviour {

    Player player;

    float CameraScrollSpeed = 25;
    int CameraScrollWidth = 15;
    float CameraMinHeight = 10;
    float CameraMaxHeight = 40;

    protected void Start ()
    {
        player = transform.root.GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!player.IsHuman)
            return;
        if (player.inputOptions.MoveCameraEnabled)
            MoveCamera();
    }

    private void MoveCamera()
    {
        float position_x = Input.mousePosition.x;
        Vector3 movement = new Vector3(HorizontalMovement(Input.mousePosition.x), 0, VerticalMovement(Input.mousePosition.y));

        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        movement.y -= CameraScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        if (destination.y > CameraMaxHeight)
            destination.y = CameraMaxHeight;
        else if (destination.y < CameraMinHeight)
            destination.y = CameraMinHeight;

        if (destination != origin)
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * CameraScrollSpeed);
    }

    private float HorizontalMovement(float position_x)
    {
        if ((position_x >= 0 && position_x < CameraScrollWidth) || Input.GetKey(KeyCode.LeftArrow))
            return -CameraScrollSpeed;
        if ((position_x <= Screen.width && position_x > Screen.width - CameraScrollWidth) || Input.GetKey(KeyCode.RightArrow))
            return CameraScrollSpeed;
        return 0;
    }

    private float VerticalMovement(float position_y)
    {
        if ((position_y >= GameWindow.BottomBarHeight && position_y < GameWindow.BottomBarHeight + CameraScrollWidth) || Input.GetKey(KeyCode.DownArrow))
            return -CameraScrollSpeed;
        if ((position_y <= Screen.height && position_y > Screen.height - CameraScrollWidth) || Input.GetKey(KeyCode.UpArrow))
            return CameraScrollSpeed;
        return 0;
    }
}
