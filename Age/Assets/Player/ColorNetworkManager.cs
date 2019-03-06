using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorNetworkManager : NetworkManager {

    int playerCount = 0;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Player player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        switch (++playerCount)
        {
            case 1:
                player.color = Color.blue;
                break;
            case 2:
                player.color = Color.red;
                break;
            case 3:
                player.color = Color.yellow;
                break;
            case 4:
                player.color = Color.green;
                break;
            default:
                player.color = Color.white;
                break;
        }
        NetworkServer.AddPlayerForConnection(conn, player.gameObject, playerControllerId);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerCount--;
        base.OnServerRemovePlayer(conn, player);
    }
}
