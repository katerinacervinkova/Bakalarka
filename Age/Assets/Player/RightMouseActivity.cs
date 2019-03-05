using UnityEngine;

public class RightMouseActivity : MouseActivity {
	
	void Update ()
    {
        if (gameState == null)
            return;
        if (Input.GetMouseButtonDown(1))
            RightMouseClick();
    }

    private void RightMouseClick()
    {

        GameObject hitObject = FindHitObject();
        Vector3 hitPoint = FindHitPoint();
        if (!hitObject || !gameState.SelectedObject)
            return;
        Selectable objectOfInterest = hitObject.GetComponent<Selectable>();
        if (hitObject.name == "Map")
            gameState.SelectedObject.RightMouseClickGround(hitPoint);
        else if (objectOfInterest != null)
            gameState.SelectedObject.RightMouseClickObject(hitObject.GetComponent<Selectable>());
    }
}
