using Prototype.NetworkLobby;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLobbyManager : LobbyManager {

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private GameObject AIplayerPrefab;

    [SerializeField]
    private List<Vector3> playerPositions;

    public int playerCount = 0;

    public int aiCount = 0;

    public override void OnLobbyStartServer()
    {
        base.OnLobbyStartServer();
        playerCount = 0;
        aiCount = 0;
        nextButton.interactable = true;
    }

    public override void OnLobbyServerPlayersReady() { }


    public void OnNextClicked()
    {
        ServerChangeScene("Menu");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (playerControllerId == 0)
            base.OnServerAddPlayer(conn, playerControllerId);
        else
        {
            aiCount++;
            GameObject player = Instantiate(AIplayerPrefab, playerPositions[playerCount], Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
        playerCount++;
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerCount--;
        if (player.playerControllerId == 0)
            base.OnServerRemovePlayer(conn, player);
        else
        {
            aiCount--;
            NetworkServer.Destroy(player.gameObject);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        GoBack();
    }

    public override void OnServerDisconnect(NetworkConnection nc)
    {
        NetworkServer.DestroyPlayersForConnection(nc);
        if (GameState.Instance)
            GameState.Instance.OnClientDisconnect();
    }

    public void GoBack()
    {
        nextButton.interactable = false;
        GoBackButton();
    }
}
