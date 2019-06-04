using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.UI;

public class CustomLobbyManager : LobbyManager {

    [SerializeField]
    private Button nextButton;

    public override void OnLobbyStartServer()
    {
        base.OnLobbyStartServer();
        nextButton.interactable = true;
    }

    public override void OnLobbyServerPlayersReady() { }

    public void OnNextClicked()
    {
        ServerChangeScene("Menu");
    }
}
