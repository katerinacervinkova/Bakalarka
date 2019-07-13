using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Selectable selectable;
    public float positionOffset;

    [SerializeField]
    private Image image;
    private bool temporary = false;
    private bool permanent = false;
    private float remainingSeconds = 0;

    private void OnEnable()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Updates the position and state of the health bar.
    /// If it is shown only temporarily, decreases remaining time and deactivates it if the time is up.
    /// </summary>
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
                if (!permanent)
                    gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Sets the position of the health bar so that it is located above its selectable.
    /// </summary>
    private void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(selectable.transform.position + positionOffset * Vector3.up);
    }

    /// <summary>
    /// Show the health bar only for a while.
    /// </summary>
    public void HideAfter(float duration = 1)
    {
        if (gameObject == null)
            return;
        gameObject.SetActive(true);
        remainingSeconds = duration;
        temporary = true;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        permanent = true;
    }

    public void Hide()
    {
        permanent = false;
        if (!temporary)
            gameObject.SetActive(false);
    }
}
