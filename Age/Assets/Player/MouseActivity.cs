using UnityEngine;

public class MouseActivity : MonoBehaviour {

    protected Player player;

    // Use this for initialization
    protected virtual void Start()
    {
        player = transform.root.GetComponent<Player>();
    }

    void Update ()
    {
		
	}
    protected bool MouseInBounds()
    {
        Vector3 mousePosition = Input.mousePosition;
        bool insideWidth = mousePosition.x >= 0 && mousePosition.x <= Screen.width;
        bool insideHeight = mousePosition.y >= player.gameWindow.BottomBorder && mousePosition.y <= Screen.height;
        return insideWidth && insideHeight;
    }

    protected GameObject FindHitObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            return hit.collider.gameObject;
        return null;
    }

    protected Vector3 FindHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) return hit.point;
        return player.gameWindow.InvalidPosition;
    }
}
