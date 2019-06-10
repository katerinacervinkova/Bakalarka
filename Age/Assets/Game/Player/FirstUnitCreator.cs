using UnityEngine;

public class FirstUnitCreator : MonoBehaviour {

    public Player player;
	
	void Update ()
    {
        if (!player.hasAuthority || player.Init())
        {
            if (player.IsHuman)
                Destroy(GameObject.Find("Loading Screen Canvas"));
            Destroy(this);
        }
	}
}
