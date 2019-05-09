using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Selectable selectable;
    public float positionOffset;

    [SerializeField]
    private Image image;
    private bool temporary = false;
    private float remainingSeconds = 0;

    private void OnEnable()
    {
        UpdatePosition();
    }
    private void Update ()
    {
        image.fillAmount = selectable.HealthValue;
        UpdatePosition();
        if (temporary)
        {
            remainingSeconds -= Time.deltaTime;
            if (remainingSeconds <= 0)
            {
                temporary = false;
                gameObject.SetActive(false);
            }
        }
    }

    private void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(selectable.transform.position + positionOffset * Vector3.up);
    }

    public void HideAfter(float duration = 1)
    {
        remainingSeconds = duration;
        temporary = true;
    }
}
