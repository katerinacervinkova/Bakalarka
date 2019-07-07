using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Main class for the Menu Scene
/// </summary>
public class MenuManager : MonoBehaviour {

    public MenuPlayer player;
    private CustomLobbyManager lobbyManager;

    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button addPlayerbutton;
    [SerializeField]
    private Button backButton;


    private void Awake()
    {
        lobbyManager = FindObjectOfType<CustomLobbyManager>();
    }

    /// <summary>
    /// Sets buttons' interactivity for server.
    /// </summary>
    public void SetServerInteractivity()
    {
        if (lobbyManager.playerCount > 1)
            playButton.interactable = true;
        addPlayerbutton.interactable = true;
        backButton.interactable = true;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void OnClickPlay()
    {
        lobbyManager.ServerChangeScene("Game");
    }

    /// <summary>
    /// Adds new player and updates buttons' interactivity.
    /// </summary>
    public void OnClickAddPlayer()
    {
        ClientScene.AddPlayer((short)(lobbyManager.aiCount + 1));
        if (lobbyManager.playerCount == 2)
            playButton.interactable = true;
        if (lobbyManager.maxPlayers == lobbyManager.playerCount)
            addPlayerbutton.interactable = false;
    }

    /// <summary>
    /// Removes the player and updates buttons' interactivity.
    /// </summary>
    /// <param name="playerControllerId">ID of the player controller the player is using</param>
    public void RemovePlayer(short playerControllerId)
    {
        ClientScene.RemovePlayer(playerControllerId);
        if (lobbyManager.playerCount == 1 && playButton != null)
            playButton.interactable = false;
        if (lobbyManager.maxPlayers - 1 == lobbyManager.playerCount && addPlayerbutton != null)
            addPlayerbutton.interactable = true;
    }

    /// <summary>
    /// Moves back to the Lobby scene.
    /// </summary>
    public void OnClickBack()
    {
        lobbyManager.ServerChangeScene("Lobby");
    }
}
