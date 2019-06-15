using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MenuPlayer : NetworkBehaviour {

    [SyncVar(hook = "OnNameChange")]
    private string Name;
    [SyncVar(hook = "OnColorChange")]
    private Color color;
    private int colorIndex;

    private PlayerRow playerRow;
    private MenuPlayerList playerList;
    private CustomLobbyManager lobbyManager;
    private MenuManager menuManager;

    [SerializeField]
    private bool isHuman;
    [SerializeField]
    private PlayerRow playerRowPrefab;
    [SerializeField]
    private Player playerPrefab;

    public override void OnStartClient()
    {
        base.OnStartClient();
        DontDestroyOnLoad(gameObject);
        lobbyManager = FindObjectOfType<CustomLobbyManager>();
        playerList = FindObjectOfType<MenuPlayerList>();
        menuManager = FindObjectOfType<MenuManager>();
        SceneManager.activeSceneChanged += SceneChanged;
        playerRow = Instantiate(playerRowPrefab, playerList.transform);
        playerRow.player = this;
    }

    public override void OnStartAuthority()
    {
        playerRow.SetInteractivity();
        if (isHuman)
            menuManager.player = this;
        if (isServer)
            menuManager.SetServerInteractivity();
        CmdSetColor();
    }

    private void SceneChanged(Scene oldScene, Scene newScene)
    {
        if (hasAuthority)
        {
            if (newScene.name == "Game")
                CmdStartGameScene();
            else if (newScene.name == "Lobby")
                RemovePlayer();
        }
    }

    public void RemovePlayer()
    {
        menuManager.RemovePlayer(playerControllerId);
    }

    public void ChangeColor()
    {
        if (hasAuthority)
            CmdChangeColor();
    }

    public void ChangeName(string Name)
    {
        if (hasAuthority)
            CmdChangeName(Name);
    }

    private void OnColorChange(Color newColor)
    {
        color = newColor;
        playerRow?.SetColor(color);
    }

    private void OnNameChange(string newName)
    {
        Name = newName;
        playerRow?.SetName(Name);
    }

    [Command]
    private void CmdSetColor()
    {
        color = playerList.GetColor(out colorIndex);
    }

    [Command]
    private void CmdStartGameScene()
    {
        Player player = Instantiate(playerPrefab, transform.position, transform.rotation);
        player.IsHuman = isHuman;
        player.Name = Name;
        player.color = color;
        NetworkServer.ReplacePlayerForConnection(connectionToClient, player.gameObject, playerControllerId);
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdChangeColor()
    {
        color = playerList.GetNextColor(ref colorIndex);
    }

    [Command]
    private void CmdChangeName(string Name)
    {
        this.Name = Name;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
        if (playerRow != null)
            Destroy(playerRow.gameObject);
        if (playerList != null)
            playerList.RemoveColor(color);
    }
}
