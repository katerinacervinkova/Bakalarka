using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Purchase purchase;
    [SerializeField]
    private RawImage image;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowToolTip(transform.position, purchase.GetDescription());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideToolTip();
    }

    public void OnClick()
    {
        purchase.Do(PlayerState.Get().SelectedObject);
    }

    public void SetPurchase(Purchase purchase)
    {
        this.purchase = purchase;
        image.texture = purchase.image;
    }
}