using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that deals with UI showing the state of the player initialization.
/// </summary>
public class PlayerRow : MonoBehaviour {

    public MenuPlayer player;

    [SerializeField]
    private Image colorImage;
    [SerializeField]
    private InputField playerName;
    [SerializeField]
    private Button removeButton;

    /// <summary>
    /// Initializes the player name.
    /// </summary>
    private void Start()
    {
        player.ChangeName(playerName.text);
    }

    /// <summary>
    /// Changes the color in the UI.
    /// </summary>
    /// <param name="color">new player color</param>
    public void SetColor(Color color)
    {
        colorImage.color = color;
    }

    /// <summary>
    /// Changes the player name in the UI.
    /// </summary>
    /// <param name="name">new player name</param>
    public void SetName(string name)
    {
        playerName.text = name;
    }

    /// <summary>
    /// Deals with player clicking the color button.
    /// </summary>
    public void OnColorClick()
    {
        player.ChangeColor();
    }

    /// <summary>
    /// Deals with player changing the player name.
    /// </summary>
    /// <param name="Name">new player name</param>
    public void OnNameChange(string Name)
    {
        player.ChangeName(Name);
    }

    /// <summary>
    /// Deals with player clicking the remove button.
    /// </summary>
    public void OnRemoveButtonClick()
    {
        player.RemovePlayer();
    }

    /// <summary>
    /// Sets the interactivity for player that has authority.
    /// </summary>
    public void SetInteractivity()
    {
        playerName.readOnly = false;
        colorImage.GetComponent<Button>().interactable = true;
        if (removeButton != null)
            removeButton.interactable = true;
    }
}
