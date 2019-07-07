using Prototype.NetworkLobby;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Class responsible for network aspects of the game
/// </summary>
public class CustomLobbyManager : LobbyManager {

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private GameObject AIplayerPrefab;

    // list of all possible player positions
    [SerializeField]
    private List<Vector3> playerPositions;

    // current count of all (human and AI) players
    public int playerCount = 0;

    // current coutn of AI players
    public int aiCount = 0;

    /// <summary>
    /// Initializes the server, buttons and player counters
    /// </summary>
    public override void OnLobbyStartServer()
    {
        base.OnLobbyStartServer();
        playerCount = 0;
        aiCount = 0;
        nextButton.interactable = true;
    }

    /// <summary>
    /// Overrides the default behaviour so that the countdown does not get started
    /// </summary>
    public override void OnLobbyServerPlayersReady() { }


    /// <summary>
    /// Advances to the menu screen
    /// </summary>
    public void OnNextClicked()
    {
        ServerChangeScene("Menu");
    }

    /// <summary>
    /// Adds the player to the server based on whether it is human or AI player
    /// </summary>
    /// <param name="conn">Network connection the player is using</param>
    /// <param name="playerControllerId">ID of the player controller the player is using</param>
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


    /// <summary>
    /// Removes the player from server based on whether it is human or AI player
    /// </summary>
    /// <param name="conn">>Network connection the player is using</param>
    /// <param name="player">Player controller the player is using</param>
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

    /// <summary>
    /// Deals with client losing connection from server by switching to network lobby.
    /// </summary>
    /// <param name="conn">Network connection the client was using</param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        GoBack();
    }

    /// <summary>
    /// Deals with server losing connection from one of its clients by destroying it and showing error message
    /// </summary>
    /// <param name="nc">Network connection the client was using</param>
    public override void OnServerDisconnect(NetworkConnection nc)
    {
        NetworkServer.DestroyPlayersForConnection(nc);
        if (GameState.Instance)
            GameState.Instance.OnClientDisconnect();
    }

    /// <summary>
    /// Switching back to network lobby.
    /// </summary>
    public void GoBack()
    {
        nextButton.interactable = false;
        GoBackButton();
    }
}
