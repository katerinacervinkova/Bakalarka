using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    public void SetServerInteractivity()
    {
        if (lobbyManager.playerCount > 1)
            playButton.interactable = true;
        addPlayerbutton.interactable = true;
        backButton.interactable = true;
    }
    public void OnClickPlay()
    {
        lobbyManager.ServerChangeScene("Game");
    }

    public void OnClickAddPlayer()
    {
        ClientScene.AddPlayer((short)(lobbyManager.aiCount + 1));
        if (lobbyManager.playerCount == 2)
            playButton.interactable = true;
        if (lobbyManager.maxPlayers == lobbyManager.playerCount)
            addPlayerbutton.interactable = false;
    }

    public void RemovePlayer(short playerControllerId)
    {
        ClientScene.RemovePlayer(playerControllerId);
        if (lobbyManager.playerCount == 1)
            playButton.interactable = false;
        if (lobbyManager.maxPlayers - 1 == lobbyManager.playerCount)
            addPlayerbutton.interactable = true;
    }

    public void OnClickBack()
    {
        lobbyManager.ServerChangeScene("Lobby");
    }
}
