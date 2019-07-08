using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for player representation in the menu
/// </summary>
public class MenuPlayer : NetworkBehaviour {

    [SyncVar(hook = "OnNameChange")]
    private string Name;
    [SyncVar(hook = "OnColorChange")]
    private Color color;
    private int colorIndex;

    private PlayerRow playerRow;
    private MenuPlayerList playerList;
    private MenuManager menuManager;

    [SerializeField]
    private bool isHuman;
    [SerializeField]
    private PlayerRow playerRowPrefab;
    [SerializeField]
    private Player playerPrefab;

    /// <summary>
    /// Inits the references.
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        DontDestroyOnLoad(gameObject);
        playerList = FindObjectOfType<MenuPlayerList>();
        menuManager = FindObjectOfType<MenuManager>();
        SceneManager.activeSceneChanged += SceneChanged;
        playerRow = Instantiate(playerRowPrefab, playerList.transform);
        playerRow.player = this;
    }

    /// <summary>
    /// Inits the interactivity and sets color.
    /// </summary>
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

    /// <summary>
    /// Removes thís player.
    /// </summary>
    public void RemovePlayer()
    {
        menuManager.RemovePlayer(playerControllerId);
    }

    /// <summary>
    /// Changes the color to the next available one.
    /// </summary>
    public void ChangeColor()
    {
        if (hasAuthority)
            CmdChangeColor();
    }

    /// <summary>
    /// Passes the name change to the server.
    /// </summary>
    /// <param name="Name">new player name</param>
    public void ChangeName(string Name)
    {
        if (hasAuthority)
            CmdChangeName(Name);
    }

    /// <summary>
    /// Reacts to the color change by updating the UI.
    /// </summary>
    /// <param name="newColor">new player color</param>
    private void OnColorChange(Color newColor)
    {
        color = newColor;
        playerRow?.SetColor(color);
    }

    /// <summary>
    /// Reacts to the name change by updating the UI.
    /// </summary>
    /// <param name="newName">new player name</param>
    private void OnNameChange(string newName)
    {
        Name = newName;
        playerRow?.SetName(Name);
    }

    /// <summary>
    /// Sets the next available color on the server.
    /// </summary>
    [Command]
    private void CmdSetColor()
    {
        color = playerList.GetColor(out colorIndex);
    }

    /// <summary>
    /// Replaces this player by actual game player.
    /// </summary>
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

    /// <summary>
    /// Changes the player color to the next available one on the server.
    /// </summary>
    [Command]
    private void CmdChangeColor()
    {
        color = playerList.GetNextColor(ref colorIndex);
    }

    /// <summary>
    /// Changes the player name on the server.
    /// </summary>
    /// <param name="Name">new player name</param>
    [Command]
    private void CmdChangeName(string Name)
    {
        this.Name = Name;
    }

    /// <summary>
    /// Destroys depending objects and listening for events.
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
        if (playerRow != null)
            Destroy(playerRow.gameObject);
        if (playerList != null)
            playerList.RemoveColor(color);
    }
}
