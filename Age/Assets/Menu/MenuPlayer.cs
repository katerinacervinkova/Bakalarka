using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MenuPlayer : NetworkBehaviour {

    [SyncVar(hook = "OnNameChange")]
    private string Name;
    [SyncVar(hook = "OnColorChange")]
    private Color color;
    private int colorIndex;

    PlayerRow playerRow;
    MenuPlayerList playerList;

    [SerializeField]
    private PlayerRow playerRowPrefab;
    [SerializeField]
    private Player playerPrefab;

    public override void OnStartClient()
    {
        base.OnStartClient();
        DontDestroyOnLoad(gameObject);
        playerList = GameObject.Find("Canvas/MenuWindow/PlayersList/PlayersArea").GetComponent<MenuPlayerList>();
        playerRow = Instantiate(playerRowPrefab, playerList.transform);
        playerRow.player = this;
        SceneManager.activeSceneChanged += SceneChanged;
    }

    public override void OnStartAuthority()
    {
        playerRow.SetInteractivity();
        if (isServer)
            FindObjectOfType<MenuManager>().SetInteractivity();
        CmdSetColor();
    }

    private void SceneChanged(Scene oldScene, Scene newScene)
    {
        if (hasAuthority)
            CmdOnSceneChanged();
    }

    [Command]
    private void CmdOnSceneChanged()
    {
        Player player = Instantiate(playerPrefab, transform.position, transform.rotation);
        player.Name = Name;
        player.color = color;
        NetworkServer.ReplacePlayerForConnection(connectionToClient, player.gameObject, 0);
        NetworkServer.Destroy(gameObject);
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
    }
}
