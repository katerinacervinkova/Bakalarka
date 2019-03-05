using UnityEngine;

public class GameWindow : MonoBehaviour
{
    static Vector3 invalidPosition = new Vector3(int.MinValue, 0, int.MinValue);
    public Vector3 InvalidPosition { get { return invalidPosition; } }

    public int BottomBorder{ get { return (int)bottomBarPanel.rect.height; } }
    public int TopBorder { get { return Screen.height; } }
    public int LeftBorder { get { return 0; } }
    public int RightBorder { get { return Screen.width; } }

    public RectTransform bottomBarPanel;
}