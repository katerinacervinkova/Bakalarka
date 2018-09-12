using UnityEngine;
using Age;

public class Images : MonoBehaviour {

    public GUISkin bottomBarSkin;

    Player player;

    const int SELECTION_NAME_HEIGHT = 15, TEXT_WIDTH = 64, TEXT_HEIGHT = 32;

    // Use this for initialization
    void Start () {
        player = transform.root.GetComponent<Player>();
    }
	
	// Update is called once per frame
	void OnGUI () {
		if (player && player.IsHuman)
        {
            GUI.skin = bottomBarSkin;
            GUI.BeginGroup(new Rect(0, Screen.height - GameWindow.BottomBarHeight, Screen.width, GameWindow.BottomBarHeight));
            GUI.Box(new Rect(0, 0, Screen.width, GameWindow.BottomBarHeight), "");
            DrawBottomBar();
            GUI.EndGroup();
        }
	}
    void DrawBottomBar()
    {
        int topPos = 4; //textLeft = 20;
        //GUI.Label(new Rect(textLeft, topPos, TEXT_WIDTH, TEXT_HEIGHT), "Food : " + player.Resources[Globals.ResourceType.Food]);
        //textLeft += TEXT_WIDTH;
        //GUI.Label(new Rect(textLeft, topPos, TEXT_WIDTH, TEXT_HEIGHT), "Wood : " + player.Resources[Globals.ResourceType.Wood]);
        //textLeft += TEXT_WIDTH;
        //GUI.Label(new Rect(textLeft, topPos, TEXT_WIDTH, TEXT_HEIGHT), "Coin : " + player.Resources[Globals.ResourceType.Coin]);

        if (player.SelectedObject)
        {
            string selectionName = player.SelectedObject.Name;
            GUI.Label(new Rect(Screen.width / 2, topPos, 150, SELECTION_NAME_HEIGHT + 10), selectionName);
        }
    }
}