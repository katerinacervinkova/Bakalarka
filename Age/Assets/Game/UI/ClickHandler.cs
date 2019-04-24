using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private CameraMovement cameraMovement;
    [SerializeField]
    private PlayerState playerState;
    [SerializeField]
    private Camera minimapCamera;
    [SerializeField]
    private RectTransform minimapButton;

    private float mapRatio;
    private Quaternion rotationMatrix;

    private void Start()
    {
        mapRatio = 2 * minimapCamera.orthographicSize / minimapButton.rect.width;
        rotationMatrix = Quaternion.Euler(-minimapButton.transform.rotation.eulerAngles);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 translatedPosition = Input.mousePosition - minimapButton.transform.position;
        Vector3 position = mapRatio * (rotationMatrix * translatedPosition);
        position = new Vector3(position.x, 0, position.y);
        if (eventData.button == PointerEventData.InputButton.Right)
            playerState.MinimapMove(position);
        else if (eventData.button == PointerEventData.InputButton.Left)
            cameraMovement.MinimapMove(position);
    }
}
