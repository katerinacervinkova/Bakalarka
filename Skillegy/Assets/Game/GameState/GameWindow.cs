using UnityEngine;

public class GameWindow : MonoBehaviour
{
    public int BottomBorder { get { return 0; } }
    public int TopBorder { get { return Screen.height; } }
    public int LeftBorder { get { return 0; } }
    public int RightBorder { get { return Screen.width; } }
}