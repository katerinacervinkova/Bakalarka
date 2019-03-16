using UnityEngine;
using UnityEngine.Networking;

public class CameraMovement : MonoBehaviour {

    public InputOptions inputOptions;
    public GameWindow gameWindow;

    readonly float ScrollSpeed = 25;
    readonly int ScrollWidth = 50;
    readonly float MinHeight = 10;
    readonly float MaxHeight = 40;

    protected void Awake ()
    {
        inputOptions = gameObject.GetComponent<InputOptions>();
        gameWindow = gameObject.GetComponent<GameWindow>();
    }


    void Update ()
    {
        if (inputOptions.MoveCameraEnabled)
            MoveCamera();
    }


    private void MoveCamera()
    {
        if (Input.mousePosition.x < gameWindow.LeftBorder || Input.mousePosition.x > gameWindow.RightBorder)
            return;
        if (Input.mousePosition.y < gameWindow.BottomBorder || Input.mousePosition.y > gameWindow.TopBorder)
            return;

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
        if ((position_x >= gameWindow.LeftBorder && position_x <  gameWindow.LeftBorder + ScrollWidth) || Input.GetKey(KeyCode.LeftArrow))
            return -ScrollSpeed;
        if ((position_x <= gameWindow.RightBorder && position_x > gameWindow.RightBorder - ScrollWidth) || Input.GetKey(KeyCode.RightArrow))
            return ScrollSpeed;
        return 0;
    }

    private float VerticalMovement(float position_y)
    {
        if ((position_y >= gameWindow.BottomBorder&& position_y < gameWindow.BottomBorder+ ScrollWidth) || Input.GetKey(KeyCode.DownArrow))
            return -ScrollSpeed;
        if ((position_y <= gameWindow.TopBorder && position_y > gameWindow.TopBorder - ScrollWidth) || Input.GetKey(KeyCode.UpArrow))
            return ScrollSpeed;
        return 0;
    }
}
