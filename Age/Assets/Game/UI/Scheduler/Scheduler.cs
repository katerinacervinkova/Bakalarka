using UnityEngine;
using UnityEngine.UI;

public class Scheduler : MonoBehaviour {

    public int index;
    public Image backgroundImage;
    public Image image;

    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.OnClickScheduler(index));
    }
}
