using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstUnitCreator : MonoBehaviour {

    public Player player;
	
	void Update ()
    {
        if (player.CreateInitialUnit())
        {
            Destroy(this);
        }
	}
}
