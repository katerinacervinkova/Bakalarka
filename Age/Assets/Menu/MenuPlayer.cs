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
        SceneManager.activeSceneChanged += SceneChanged;
        playerRow = Instantiate(playerRowPrefab, playerList.transform);
        playerRow.player = this;
    }

    public override void OnStartAuthority()
    {
        playerRow.SetInteractivity();
        var manager = FindObjectOfType<MenuManager>();
        if (isHuman)
            manager.player = this;
        if (isServer)
            manager.SetInteractivity();
        CmdSetColor();
    }

    private void SceneChanged(Scene oldScene, Scene newScene)
    {
        if (hasAuthority)
            CmdOnSceneChanged();
    }

    public void AddPlayer(out bool maxPlayers)
    {
        ClientScene.AddPlayer((short)connectionToServer.playerControllers.Count);
        maxPlayers = lobbyManager.maxPlayers <= lobbyManager.playerCount;
    }

    public void RemovePlayer()
    {
        ClientScene.RemovePlayer(playerControllerId);
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
    private void CmdOnSceneChanged()
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
