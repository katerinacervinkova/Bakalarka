using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour {

    public MenuPlayer player;

    [SerializeField]
    private Image colorImage;
    [SerializeField]
    private InputField playerName;


    public void SetColor(Color color)
    {
        colorImage.color = color;
    }

    public void SetName(string name)
    {
        playerName.text = name;
    }

    public void OnColorClick()
    {
        player.ChangeColor();
    }

    public void SetInteractivity()
    {
        playerName.readOnly = false;
        colorImage.GetComponent<Button>().interactable = true;
    }
}
