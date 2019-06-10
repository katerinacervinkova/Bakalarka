using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour {

    public MenuPlayer player;

    [SerializeField]
    private Image colorImage;
    [SerializeField]
    private InputField playerName;
    [SerializeField]
    private Button removeButton;

    private void Start()
    {
        player.ChangeName(playerName.text);
    }

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

    public void OnNameChange(string Name)
    {
        player.ChangeName(Name);
    }

    public void OnRemoveButtonClick()
    {
        player.RemovePlayer();
    }

    public void SetInteractivity()
    {
        playerName.readOnly = false;
        colorImage.GetComponent<Button>().interactable = true;
        if (removeButton != null)
            removeButton.interactable = true;
    }
}
