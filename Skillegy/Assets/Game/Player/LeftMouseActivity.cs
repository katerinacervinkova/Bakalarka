using UnityEngine;
using System;

public class LeftMouseActivity : MouseActivity {

    // UI for the square used for selecting multiple units
    [SerializeField]
    private RectTransform selectionSquare;
    private Vector3 squareStartPosition = Vector3.zero;

    private bool isClicking = false;
    private readonly float maxClickTime = 0.3f;
    private float lastClickTime = 0;

    private GameObject hitObject = null;
    private Vector3 hitPoint = Vector3.zero;

    private void Update ()
    {
        // game has not started yet or the classical input is paused by another window
        if (PlayerState.Get() == null || BuildingWindowShown)
        {
            selectionSquare.gameObject.SetActive(false);
            return;
        }
        // classical input is paused by placing a building
        if (PlayerState.Get().BuildingToBuild != null)
        {
            // "r" key resets the purchase of the building
            if (Input.GetKeyDown("r"))
            {
                PlayerState.Get().ResetBuildingToBuild();
                return;
            }
            // moving the mouses moves the building as well
            Vector3 hitPoint = FindHitPoint();
            if (!float.IsPositiveInfinity(hitPoint.x))
            {
                hitPoint.y = 0;
                PlayerState.Get().MoveBuildingToBuild(hitPoint);
            }
        }

        // handles mouse click or release
        if (Input.GetMouseButtonDown(0))
            LeftMouseDown();
        else if (Input.GetMouseButtonUp(0))
        {
            if (isClicking)
                LeftMouseRelease();
            selectionSquare.gameObject.SetActive(false);
        }
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
        if (float.IsPositiveInfinity(hitPoint.x))
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
        // if the player is currently placing a building, place it
        if (PlayerState.Get().BuildingToBuild != null && !float.IsPositiveInfinity(hitPoint.x))
        {
            PlayerState.Get().PlaceBuilding();
            return;
        }
        if (!hitObject || float.IsPositiveInfinity(hitPoint.x))
            return;
        // it the player has clicked on the ground, deselect currently selected object
        if (hitObject.name == "Map")
            PlayerState.Get().Deselect();
        else
        {
            // select the object the player has chosen
            Selectable selectedObject = hitObject.transform.GetComponent<Selectable>();
            if (!selectedObject)
                return;
            PlayerState.Get().Select(selectedObject);
        }
    }
    private void LeftMouseDrag()
    {
        // select all player's units within given rectangle
        inputOptions.MoveCameraEnabled = true;
        selectionSquare.gameObject.SetActive(false);

        Vector3 topLeft, bottomRight;
        RectangleCoordinates(out topLeft, out bottomRight);

        PlayerState.Get().Select(unit => IsWithinRectangle(topLeft, bottomRight, unit.transform) && unit.isActiveAndEnabled);
    }

    private void DrawRectangle()
    {
        // draws the UI selection rectangle and disables camera movement
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
