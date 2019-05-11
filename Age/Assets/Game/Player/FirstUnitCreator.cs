using UnityEngine;

public class FirstUnitCreator : MonoBehaviour {

    public Player player;
	
	void Update ()
    {
        if (!player.hasAuthority || player.Init())
            Destroy(this);
	}
}
