using UnityEngine;

public class RightMouseActivity : MouseActivity {
	
	void Update ()
    {
        if (playerState == null)
            return;
        if (Input.GetMouseButtonDown(1))
            RightMouseClick();
    }

    private void RightMouseClick()
    {

        GameObject hitObject = FindHitObject();
        Vector3 hitPoint = FindHitPoint();
        if (!hitObject || !playerState.SelectedObject)
            return;
        Selectable objectOfInterest = hitObject.GetComponent<Selectable>();
        if (hitObject.name == "Map")
            playerState.SelectedObject.RightMouseClickGround(hitPoint);
        else if (objectOfInterest != null)
            playerState.SelectedObject.RightMouseClickObject(hitObject.GetComponent<Selectable>());
    }
}
