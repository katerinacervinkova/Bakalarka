using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class that handles click on the minimap.
/// </summary>
public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private CameraMovement cameraMovement;
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

    /// <summary>
    /// Determines the world position of the minimap click and handles the click.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 translatedPosition = Input.mousePosition - minimapButton.transform.position;
        Vector3 position = mapRatio * (rotationMatrix * translatedPosition);
        position = new Vector3(position.x, 0, position.y);
        if (eventData.button == PointerEventData.InputButton.Right)
            PlayerState.Get().RightClickMinimap(position);
        else if (eventData.button == PointerEventData.InputButton.Left)
            cameraMovement.MinimapMove(position);
    }
}
