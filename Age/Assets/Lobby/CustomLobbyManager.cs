using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyManager : NetworkLobbyManager
{
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        var cc = lobbyPlayer.GetComponent<ColorControl>();
        var player = gamePlayer.GetComponent<Player>();
        player.color = cc.color;
        return true;
    }
}