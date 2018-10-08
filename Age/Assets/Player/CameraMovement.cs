using UnityEngine;

public class CameraMovement : MonoBehaviour {

    Player player;

    float ScrollSpeed = 25;
    int ScrollWidth = 50;
    float MinHeight = 10;
    float MaxHeight = 40;

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
        Vector3 movement = new Vector3(HorizontalMovement(Input.mousePosition.x), 0, VerticalMovement(Input.mousePosition.y));

        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        movement.y -= ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        if (destination.y > MaxHeight)
            destination.y = MaxHeight;
        else if (destination.y < MinHeight)
            destination.y = MinHeight;

        if (destination != origin)
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ScrollSpeed);
    }

    private float HorizontalMovement(float position_x)
    {
        if ((position_x >= player.gameWindow.LeftBorder && position_x <  player.gameWindow.LeftBorder + ScrollWidth) || Input.GetKey(KeyCode.LeftArrow))
            return -ScrollSpeed;
        if ((position_x <= player.gameWindow.RightBorder && position_x > player.gameWindow.RightBorder - ScrollWidth) || Input.GetKey(KeyCode.RightArrow))
            return ScrollSpeed;
        return 0;
    }

    private float VerticalMovement(float position_y)
    {
        if ((position_y >= player.gameWindow.BottomBorder&& position_y < player.gameWindow.BottomBorder+ ScrollWidth) || Input.GetKey(KeyCode.DownArrow))
            return -ScrollSpeed;
        if ((position_y <= player.gameWindow.TopBorder && position_y > player.gameWindow.TopBorder - ScrollWidth) || Input.GetKey(KeyCode.UpArrow))
            return ScrollSpeed;
        return 0;
    }
}
