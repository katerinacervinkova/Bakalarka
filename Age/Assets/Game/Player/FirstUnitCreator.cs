using UnityEngine;

public class FirstUnitCreator : MonoBehaviour {

    public Player player;
	
	void Update ()
    {
        if (player.Init())
            Destroy(this);
	}
}
