using Prototype.NetworkLobby;
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

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player;
        if (playerControllerId == 0)
            base.OnServerAddPlayer(conn, playerControllerId);
        else
        {
            player = Instantiate(AIplayerPrefab, playerPositions[playerCount], Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
        playerCount++;
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerCount--;
        if (player.playerControllerId == 0)
            base.OnServerRemovePlayer(conn, player);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
    }

    public override void OnServerDisconnect(NetworkConnection nc)
    {
        NetworkServer.DestroyPlayersForConnection(nc);
    }
}
