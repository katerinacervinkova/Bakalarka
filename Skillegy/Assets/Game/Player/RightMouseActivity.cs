using UnityEngine;

public class RightMouseActivity : MouseActivity {
	
	void Update ()
    {
        if (PlayerState.Get() == null || BuildingWindowShown)
            return;
        if (Input.GetMouseButtonDown(1))
            RightMouseClick();
    }

    private void RightMouseClick()
    {
        if (inputOptions.MouseOverUI)
            return;
        GameObject hitObject = FindHitObject();
        Vector3 hitPoint = FindHitPoint();
        if (!hitObject || !PlayerState.Get().SelectedObject)
            return;
        Selectable objectOfInterest = hitObject.GetComponent<Selectable>();
        if (hitObject.name == "Map")
            PlayerState.Get().SelectedObject.RightMouseClickGround(hitPoint);
        else if (objectOfInterest != null)
            PlayerState.Get().SelectedObject.RightMouseClickObject(hitObject.GetComponent<Selectable>());
    }
}
