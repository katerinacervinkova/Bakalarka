using UnityEngine;

public class RightMouseActivity : MouseActivity {
	
	void Update ()
    {
        if (PlayerState.Instance == null)
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
        if (!hitObject || !PlayerState.Instance.SelectedObject)
            return;
        Selectable objectOfInterest = hitObject.GetComponent<Selectable>();
        if (hitObject.name == "Map")
            PlayerState.Instance.SelectedObject.RightMouseClickGround(hitPoint);
        else if (objectOfInterest != null)
            PlayerState.Instance.SelectedObject.RightMouseClickObject(hitObject.GetComponent<Selectable>());
    }
}
