using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public MenuPlayer player;

    public void SetInteractivity()
    {
        transform.Find("PlayButton").GetComponent<Button>().interactable = true;
        transform.Find("AddPlayerButton").GetComponent<Button>().interactable = true;
        transform.Find("BackButton").GetComponent<Button>().interactable = true;
    }
    public void OnClickPlay()
    {
        GameObject.Find("LobbyManager").GetComponent<NetworkLobbyManager>().ServerChangeScene("Game");
    }

    public void OnClickAddPlayer()
    {
        bool maxPlayers;
        player.AddPlayer(out maxPlayers);
        if (maxPlayers)
            transform.Find("AddPlayerButton").GetComponent<Button>().interactable = false;
    }
}
