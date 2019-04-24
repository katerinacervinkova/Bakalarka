using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class LeftMouseActivity : MouseActivity {

    public InputOptions inputOptions;

    public RectTransform selectionSquare;
    readonly float maxClickTime = 0.3f;
    float lastClickTime = 0;
    GameObject hitObject = null;
    Vector3 hitPoint = Vector3.zero;
    Vector3 squareStartPosition = Vector3.zero;
    bool isClicking = false;
    private int fingerID = -1;

    protected override void Awake()
    {
        base.Awake();
        #if !UNITY_EDITOR
            fingerID = 0; 
        #endif
        inputOptions = gameObject.GetComponent<InputOptions>();
    }

    private void Update ()
    {
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
        if (EventSystem.current.IsPointerOverGameObject(fingerID))
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
        var selectedUnits = new List<Unit>();

        Vector3 topLeft, bottomRight;
        RectangleCoordinates(out topLeft, out bottomRight);

        PlayerState.Instance.Select(unit => IsWithinRectangle(topLeft, bottomRight, unit.transform));
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
        Vector3 squareEndPosition = Input.mousePosition;

        squareEndPosition.x = Math.Max(gameWindow.LeftBorder, squareEndPosition.x);
        squareEndPosition.y = Math.Max(gameWindow.BottomBorder, squareEndPosition.y);
        squareEndPosition.x = Math.Min(gameWindow.RightBorder, squareEndPosition.x);
        squareEndPosition.y = Math.Min(gameWindow.TopBorder, squareEndPosition.y);

        topLeft = new Vector3(Math.Min(squareStartPosition.x, squareEndPosition.x), Math.Min(squareStartPosition.y, squareEndPosition.y), 0);
        bottomRight = new Vector3(Math.Max(squareStartPosition.x, squareEndPosition.x), Math.Max(squareStartPosition.y, squareEndPosition.y), 0);
    }

    bool IsWithinRectangle(Vector3 topLeft, Vector3 bottomRight, Transform transform)
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
