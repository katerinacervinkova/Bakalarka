using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorNetworkManager : NetworkManager {

    public List<Player> players = new List<Player>();

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Player player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        players.Add(player);
        switch (players.Count)
        {
            case 1:
                player.color = Color.blue;
                player.Name = "Modrý";
                break;
            case 2:
                player.color = Color.red;
                player.Name = "Červený";
                break;
            case 3:
                player.color = Color.yellow;
                player.Name = "Žlutý";
                break;
            case 4:
                player.color = Color.green;
                player.Name = "Zelený";
                break;
            default:
                player.color = Color.white;
                player.Name = "Bílý";
                break;
        }
        NetworkServer.AddPlayerForConnection(conn, player.gameObject, playerControllerId);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        players.Remove(player.gameObject.GetComponent<Player>());
        base.OnServerRemovePlayer(conn, player);
    }
}
