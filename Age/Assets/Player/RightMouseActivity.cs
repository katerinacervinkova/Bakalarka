using UnityEngine;

public class RightMouseActivity : MouseActivity {

	protected override void Start ()
    {
        base.Start();
	}
	
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
        if (player != player.SelectedObject.owner)
            return;
        Selectable objectOfInterest = hitObject.GetComponent<Selectable>();
        if (hitObject.name == "Map")
            player.SelectedObject.RightMouseClickGround(hitPoint);
        else if (objectOfInterest != null)
            player.SelectedObject.RightMouseClickObject(hitObject.GetComponent<Selectable>());
    }
}
