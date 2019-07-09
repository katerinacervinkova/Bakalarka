using UnityEngine;

public class MouseActivity : MonoBehaviour {

    protected GameWindow gameWindow;
    protected InputOptions inputOptions;

    protected virtual void Awake()
    {
        gameWindow = gameObject.GetComponent<GameWindow>();
        inputOptions = gameObject.GetComponent<InputOptions>();
    }

    protected bool BuildingWindowShown => UIManager.Instance.BuildingWindowShown != null;

    protected bool MouseInBounds()
    {
        Vector3 mousePosition = Input.mousePosition;
        bool insideWidth = mousePosition.x >= gameWindow.LeftBorder && mousePosition.x <= gameWindow.RightBorder;
        bool insideHeight = mousePosition.y >= gameWindow.BottomBorder && mousePosition.y <= gameWindow.TopBorder;
        return insideWidth && insideHeight;
    }

    protected GameObject FindHitObject()
    {
        RaycastHit hit;
        if (FindRaycastHit(out hit))
            return hit.collider.gameObject;
        return null;
    }

    protected Vector3 FindHitPoint()
    {
        RaycastHit hit;
        if (FindRaycastHit(out hit))
            return hit.point;
        return Vector3.positiveInfinity;
    }

    private bool FindRaycastHit(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            return true;
        return false;
    }
}
