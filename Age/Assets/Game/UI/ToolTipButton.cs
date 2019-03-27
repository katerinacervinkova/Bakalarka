using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Transaction transaction;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowToolTip(transform.position, transaction.GetDescription());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideToolTip();
    }

}
