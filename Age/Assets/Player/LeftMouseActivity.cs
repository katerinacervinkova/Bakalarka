using UnityEngine;
using Age;
using System;
using System.Collections.Generic;

public class LeftMouseActivity : MouseActivity {

    public RectTransform selectionSquare;

    float maxClickTime = 0.2f;
    float lastClickTime = 0;
    GameObject hitObject = null;
    Vector3 hitPoint = Vector3.zero;
    Vector3 squareStartPosition = Vector3.zero;
    bool isClicking = false;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
    }
	
	// Update is called once per frame
	private void Update ()
    {
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
        if (hitPoint == GameWindow.InvalidPosition)
            return;
        isClicking = true;
        squareStartPosition = Input.mousePosition;
    }

    private void LeftMouseRelease()
    {
        isClicking = false;
        if (player.SelectedObject)
            player.SelectedObject.SetSelection(false, player);
        if (Time.time - lastClickTime < maxClickTime)
            LeftMouseClick();
        else
            LeftMouseDrag();

    }
    private void LeftMouseClick()
    {
        if (!hitObject || hitPoint == GameWindow.InvalidPosition)
            return;
        if (hitObject.name == "Map")
            player.SelectedObject = null;
        else
        {
            Selectable worldObject = hitObject.transform.GetComponent<Selectable>();
            if (!worldObject)
                return;
            player.SelectedObject = worldObject;
            worldObject.SetSelection(true, player);
        }
    }
    private void LeftMouseDrag()
    {
        player.inputOptions.MoveCameraEnabled = true;
        selectionSquare.gameObject.SetActive(false);
        var selectedUnits = new List<Unit>();

        Vector3 topLeft, bottomRight;
        RectangleCoordinates(out topLeft, out bottomRight);

        foreach (Unit unit in player.units)
            if (IsWithinRectangle(topLeft, bottomRight, unit.transform))
                selectedUnits.Add(unit);

        if (selectedUnits.Count > 0)
            player.SelectedObject = null;

        Regiment regiment = player.factory.CreateRegiment(player, selectedUnits);
        player.SelectedObject = regiment;
        regiment.SetSelection(true, player);
    }

    private void DrawRectangle()
    {
        if (Time.time - lastClickTime < maxClickTime)
            return;

        player.inputOptions.MoveCameraEnabled = false;
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

        squareEndPosition.x = Math.Max(GameWindow.LeftBorder, squareEndPosition.x);
        squareEndPosition.y = Math.Max(GameWindow.BottomBorder, squareEndPosition.y);
        squareEndPosition.x = Math.Min(GameWindow.RightBorder, squareEndPosition.x);
        squareEndPosition.y = Math.Min(GameWindow.TopBorder, squareEndPosition.y);

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
