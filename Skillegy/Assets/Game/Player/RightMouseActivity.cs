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
        if (inputOptions.MouseOverUI || PlayerState.Get().SelectedObject == null)
            return;

        GameObject hitObject = FindHitObject();
        Vector3 hitPoint = FindHitPoint();
        if (hitObject == null)
            return;

        if (hitObject.name == "Map")
            PlayerState.Get().SelectedObject.RightMouseClickGround(hitPoint);

        Selectable selectable = hitObject.GetComponent<Selectable>();
        if (selectable != null)
            PlayerState.Get().SelectedObject.RightMouseClickObject(selectable);
    }
}
