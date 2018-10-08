using UnityEngine;

public class BottomBar : MonoBehaviour {

    public GUISkin bottomBarSkin;

    Player player;

    const int TEXT_HEIGHT = 10;

    // Use this for initialization
    void Start () {
        player = transform.root.GetComponent<Player>();
    }
	
	// Update is called once per frame
	void OnGUI () {
		if (player && player.IsHuman)
        {
            //GUI.skin = bottomBarSkin;
            //GUI.BeginGroup(new Rect(0, Screen.height - GameWindow.BottomBarHeight, Screen.width, GameWindow.BottomBarHeight));
            //GUI.Box(new Rect(0, 0, Screen.width, GameWindow.BottomBarHeight), "");
            //Draw();
            //GUI.EndGroup();
        }
	}
    //void Draw()
    //{
    //    int leftPos = Screen.width / 4;
    //    if (player.SelectedObject)
    //    {
    //        string selectionName = player.SelectedObject.Name;
    //        GUI.Label(new Rect(leftPos, 0, 150, 20), selectionName);
    //        player.SelectedObject.DrawBottomBar(leftPos + 100);
    //    }
    //}
}