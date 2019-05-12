using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTippedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [TextArea]
    [SerializeField]
    private string description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowToolTip(transform.position, description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideToolTip();
    }
}
