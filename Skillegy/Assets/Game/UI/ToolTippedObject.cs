using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class responsible for showing the tooltip to the given object.
/// </summary>
public class ToolTippedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [TextArea]
    [SerializeField]
    private string description;

    private bool isOver = false;

    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            description = value;
            if (isOver)
                UIManager.Instance.ShowToolTip(transform.position, description);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowToolTip(transform.position, description);
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideToolTip();
        isOver = false;
    }
    public void OnDisable()
    {
        if (isOver)
        {
            UIManager.Instance.HideToolTip();
            isOver = false;
        }
    }

    public void OnEnable()
    {
        if (isOver)
            UIManager.Instance.ShowToolTip(transform.position, description);
    }
}
