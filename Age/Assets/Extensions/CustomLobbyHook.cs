using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
    }
}
