using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Age;

public class RightMouseActivity : MouseActivity {

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetMouseButtonDown(1))
            RightMouseClick();
    }

    private void RightMouseClick()
    {

        GameObject hitObject = FindHitObject();
        Vector3 hitPoint = FindHitPoint();
        if (!hitObject || !player.SelectedObject)
            return;
        // player is not the owner
        if (player != player.SelectedObject.owner)
            return;
        if (hitObject.name == "Map")
            player.SelectedObject.RightMouseClick(hitObject, hitPoint);

    }
}
