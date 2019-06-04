using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuManager : NetworkBehaviour {

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
}
