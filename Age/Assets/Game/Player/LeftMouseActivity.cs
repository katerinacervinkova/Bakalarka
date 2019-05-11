using UnityEngine;
using System;

public class LeftMouseActivity : MouseActivity {

    [SerializeField]
    private RectTransform selectionSquare;
    private readonly float maxClickTime = 0.3f;
    private float lastClickTime = 0;
    private GameObject hitObject = null;
    private Vector3 hitPoint = Vector3.zero;
    private Vector3 squareStartPosition = Vector3.zero;
    private bool isClicking = false;

    private void Update ()
    {
        if (PlayerState.Instance == null || BuildingWindowShown)
        {
            selectionSquare.gameObject.SetActive(false);
            return;
        }
        if (PlayerState.Instance.BuildingToBuild != null)
        {
            Vector3 hitPoint = FindHitPoint();
            hitPoint.y = 0;
            PlayerState.Instance.MoveBuildingToBuild(hitPoint);
        }

        if (!isClicking && !MouseInBounds())
            return;
        if (Input.GetMouseButtonUp(0))
            LeftMouseRelease();
        if (Input.GetMouseButtonDown(0))
            LeftMouseDown();
    }

    private void OnGUI()
    {
        if (isClicking)
            DrawRectangle();
    }

    private void LeftMouseDown()
    {
        lastClickTime = Time.time;
        hitObject = FindHitObject();
        hitPoint = FindHitPoint();
        if (hitPoint == gameWindow.InvalidPosition)
            return;
        isClicking = true;
        squareStartPosition = Input.mousePosition;
    }

    private void LeftMouseRelease()
    {
        isClicking = false;
        if (Time.time - lastClickTime < maxClickTime)
            LeftMouseClick();
        else
            LeftMouseDrag();

    }
    private void LeftMouseClick()
    {
        if (inputOptions.MouseOverUI)
            return;
        if (PlayerState.Instance.SelectedObject && PlayerState.Instance.BuildingToBuild == null)
            PlayerState.Instance.Deselect();
        if (PlayerState.Instance.BuildingToBuild != null && hitPoint != gameWindow.InvalidPosition)
        {
            PlayerState.Instance.PlaceBuilding();
            return;
        }
        if (!hitObject || hitPoint == gameWindow.InvalidPosition)
            return;
        if (hitObject.name == "Map")
            PlayerState.Instance.SelectedObject = null;
        else
        {
            Selectable selectedObject = hitObject.transform.GetComponent<Selectable>();
            if (!selectedObject)
                return;
            PlayerState.Instance.Select(selectedObject);
        }
    }
    private void LeftMouseDrag()
    {
        inputOptions.MoveCameraEnabled = true;
        selectionSquare.gameObject.SetActive(false);

        Vector3 topLeft, bottomRight;
        RectangleCoordinates(out topLeft, out bottomRight);

        PlayerState.Instance.Select(unit => IsWithinRectangle(topLeft, bottomRight, unit.transform) && unit.isActiveAndEnabled);
    }

    private void DrawRectangle()
    {
        if (Time.time - lastClickTime < maxClickTime)
            return;

        inputOptions.MoveCameraEnabled = false;
        if (!selectionSquare.gameObject.activeInHierarchy)
            selectionSquare.gameObject.SetActive(true);

        Vector3 topLeft, bottomRight;
        RectangleCoordinates(out topLeft, out bottomRight);

        var width = bottomRight.x - topLeft.x;
        var heigth = bottomRight.y - topLeft.y;

        var squarePosition = (topLeft + bottomRight) / 2;

        selectionSquare.position = squarePosition;
        selectionSquare.sizeDelta = new Vector2(width, heigth);
    }

    private void RectangleCoordinates(out Vector3 topLeft, out Vector3 bottomRight)
    {
        Vector3 endPos = Input.mousePosition;

        endPos.x = Mathf.Clamp(endPos.x, gameWindow.LeftBorder, gameWindow.RightBorder);
        endPos.y = Mathf.Clamp(endPos.y, gameWindow.BottomBorder, gameWindow.TopBorder);

        topLeft = new Vector3(Math.Min(squareStartPosition.x, endPos.x), Math.Min(squareStartPosition.y, endPos.y), 0);
        bottomRight = new Vector3(Math.Max(squareStartPosition.x, endPos.x), Math.Max(squareStartPosition.y, endPos.y), 0);
    }

    private bool IsWithinRectangle(Vector3 topLeft, Vector3 bottomRight, Transform transform)
    {
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPos.x < topLeft.x)
            return false;
        if (screenPos.y < topLeft.y)
            return false;
        if (screenPos.x > bottomRight.x)
            return false;
        if (screenPos.y > bottomRight.y)
            return false;
        return true;
    }
}
