using Pathfinding;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Main class for the player. Does most of operations, that have to be synchronized, on the server.
/// </summary>
public class Player : NetworkBehaviour
{
    [SyncVar]
    public bool IsHuman = true;

    [SyncVar]
    public string Name;
    [SyncVar]
    public Color color;

    public Factory factory;
    public VictoryCondition victoryCondition;

    [SyncVar]
    public bool InGame = false;
    private GameObject endGameCanvas;

    /// <summary>
    /// Creates player state, player purchases and moves the camera.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        PlayerState.Set(playerControllerId, factory.CreatePlayerState());
        PlayerState.Get(playerControllerId).playerPurchases = factory.CreatePlayerPurchases();
        if (IsHuman)
            Camera.main.transform.parent.position = transform.position;
    }

    /// <summary>
    /// If the scene and player is ready, inits the fog of war and creates the initial unit.
    /// </summary>
    /// <returns>true if succeeded</returns>
    public bool Init()
    {
        if (!hasAuthority || (connectionToClient != null && !connectionToClient.isReady) || GameState.Instance == null)
            return false;
        GameState.Instance.SetVisibilitySquares(playerControllerId, factory.CreateVisibilitySquares());

        CmdCreateInitialUnit(transform.position, transform.position);
        return true;
    }

    /// <summary>
    /// Start the game as soon as the initial unit is created.
    /// </summary>
    public void StartTheGame()
    {
        PlayerState.Get(playerControllerId).Population = 1;
        if (!IsHuman)
        {
            AIPlayer aiPlayer = factory.CreateAIPlayer();
            SimpleAI AI = factory.CreateAI();
            aiPlayer.playerState = PlayerState.Get(playerControllerId);
            aiPlayer.gameState = GameState.Instance;
            AI.aiPlayer = aiPlayer;
        }
        else
        {
            endGameCanvas = GameObject.Find("EndGameCanvas");
            CustomLobbyManager manager = FindObjectOfType<CustomLobbyManager>();
            endGameCanvas.transform.Find("MenuButton").GetComponent<Button>().onClick.AddListener(manager.GoBack);
            endGameCanvas.SetActive(false);
        }
        CmdChangeInGame(true);
    }

    /// <summary>
    /// Check the victory condition in each frame.
    /// </summary>
    private void Update()
    {
        if (victoryCondition == null)
        {
            var vc = GameObject.Find("VictoryCondition");
            if (vc != null)
            {
                victoryCondition = vc.GetComponent<VictoryCondition>();
                victoryCondition.players.Add(this);
            }
        }
        if (hasAuthority && InGame && victoryCondition != null)
        {
            if (victoryCondition.PlayerMeetsLosingConditions(this))
                Lose();
            else if (victoryCondition.PlayerMeetsConditions(this))
                Win();
        }
    }

    /// <summary>
    /// Shows the losing screen and ends the game.
    /// </summary>
    public void Lose()
    {
        if (IsHuman)
            endGameCanvas.transform.Find("Text").GetComponent<Text>().text = "You lose!";
        EndGame();
    }

    /// <summary>
    /// Shows the winning screen and ends the game.
    /// </summary>
    public void Win()
    {
        if (IsHuman)
            endGameCanvas.transform.Find("Text").GetComponent<Text>().text = "You win!";
        EndGame();
    }

    /// <summary>
    /// Ends the game for this player and removes UI and fog of war
    /// </summary>
    public void EndGame()
    {
        CmdChangeInGame(false);
        if (IsHuman)
        {
            endGameCanvas.SetActive(true);
            ((HumanVisibilitySquares)GameState.Instance.GetSquares(playerControllerId)).SeeEverything();
            Destroy(GameObject.Find("MainCanvas"));
        }
    }

    /// <summary>
    /// Makes unit exit the building if it is currently inhabiting it.
    /// </summary>
    /// <param name="unit">unit to exit</param>
    /// <param name="building">building the unit is about to leave</param>
    public void ExitBuilding(Unit unit, Building building)
    {
        if (unit != null && building != null)
            CmdExitBuilding(unit.netId, building.FrontPosition, building.DefaultDestination);
    }

    /// <summary>
    /// Makes the attacker attack the target.
    /// </summary>
    /// <param name="attacker">attacker to attack</param>
    /// <param name="target">target to be attacked</param>
    public void Attack(Selectable attacker, Selectable target) => CmdAttack(attacker.netId, target.netId);
    /// <summary>
    /// Makes unit enter the building.
    /// </summary>
    /// <param name="unit">unit to enter the building</param>
    /// <param name="building">building to be entered</param>
    public void EnterBuilding(Unit unit, Building building) => CmdEnterBuilding(unit.netId);
    /// <summary>
    /// Takes given amount away from given resource
    /// </summary>
    /// <param name="amount">amount to be gathered</param>
    /// <param name="resource">resource to be gathered from</param>
    public void Gather(float amount, Resource resource) => CmdGather(amount, resource.netId);
    /// <summary>
    /// Changes the unit's attribute.
    /// </summary>
    /// <param name="unit">unit which the attribute belongs to</param>
    /// <param name="attEnum">type of the attribute</param>
    /// <param name="value">new value for the attribute</param>
    public void ChangeAttribute(Unit unit, SkillEnum attEnum, float value) => CmdChangeAttribute(unit.netId, attEnum, value);
    /// <summary>
    /// Creates the temporary building of given type.
    /// </summary>
    /// <param name="buildingType">type of the building</param>
    public void CreateTempBuilding(BuildingEnum buildingType) => CmdCreateTempBuilding(buildingType);
    /// <summary>
    /// Creates unit from the given building.
    /// </summary>
    /// <param name="building">building to create the unit</param>
    public void CreateUnit(Building building) => CmdCreateUnit(building.FrontPosition, building.DefaultDestination);
    /// <summary>
    /// Places building onto its current position and makes it visible for all clients.
    /// </summary>
    /// <param name="temporaryBuilding"></param>
    public void PlaceBuilding(TemporaryBuilding temporaryBuilding) => CmdPlaceBuilding(temporaryBuilding.transform.position, temporaryBuilding.netId);
    /// <summary>
    /// Changes health of the selectable.
    /// </summary>
    /// <param name="selectable">selectable whose health is changing</param>
    /// <param name="value">new value of health</param>
    public void ChangeHealth(Selectable selectable, float value) => CmdChangeHealth(selectable.netId, value);
    /// <summary>
    /// Destroys given object.
    /// </summary>
    /// <param name="selectedObject">object to be destroyed</param>
    public void DestroySelectedObject(Selectable selectedObject) => CmdDestroy(selectedObject.netId);

    /// <summary>
    /// Finds the nearest walkable position to the given position.
    /// </summary>
    /// <param name="position">position to start with</param>
    /// <returns> the nearest walkable position</returns>
    private Vector3 NearestWalkable(Vector3 position)
    {
        NNConstraint nodeConstraint = new NNConstraint
        {
            constrainWalkability = true,
            walkable = true
        };
        if (AstarPath.active == null)
            return new Vector3();
        return AstarPath.active.GetNearest(position, nodeConstraint).position;
    }

    // bunch of commands that perform given operations on the server
    // and call the functions that synchronize them with clients

    [Command]
    private void CmdChangeInGame(bool inGame)
    {
        InGame = inGame;
    }

    [Command]
    private void CmdChangeHealth(NetworkInstanceId selectableId, float value)
    {
        if (NetworkServer.objects.ContainsKey(selectableId))
        {
            Selectable selectable = NetworkServer.objects[selectableId].GetComponent<Selectable>();
            selectable.Health = Mathf.Clamp(value, 0, selectable.MaxHealth);
            if (selectable.Health == 0)
                CmdDestroy(selectableId);
        }
    }

    [Command]
    private void CmdAttack(NetworkInstanceId attackerId, NetworkInstanceId targetId)
    {
        GameState.Instance.RpcAttack(attackerId, targetId);
    }

    [Command]
    private void CmdEnterBuilding(NetworkInstanceId unitId)
    {
        GameState.Instance.RpcEnterBuilding(unitId);
    }

    [Command]
    private void CmdExitBuilding(NetworkInstanceId unitId, Vector3 position, Vector3 destination)
    {
        Vector3 pos = NearestWalkable(position);
        if (position == destination)
            GameState.Instance.RpcExitBuilding(unitId, pos);
        else
            GameState.Instance.RpcExitBuildingDestination(unitId, pos, destination);
    }

    [Command]
    private void CmdCreateUnit(Vector3 position, Vector3 destination)
    {
        Unit unit = factory.CreateUnit(NearestWalkable(position), netId);
        NetworkServer.SpawnWithClientAuthority(unit.gameObject, gameObject);
        if (destination != position)
            unit.SetJob(new JobGo(destination));
    }

    [Command]
    private void CmdCreateInitialUnit(Vector3 position, Vector3 destination)
    {
        Unit unit = factory.CreateUnit(NearestWalkable(position), netId);
        unit.Building = 7;
        NetworkServer.SpawnWithClientAuthority(unit.gameObject, gameObject);
        if (destination != position)
            unit.SetJob(new JobGo(destination));
    }

    [Command]
    private void CmdCreateTempBuilding(BuildingEnum buildingType)
    {
        var tempBuilding = factory.CreateTemporaryMainBuilding(netId, buildingType);
        NetworkServer.SpawnWithClientAuthority(tempBuilding.gameObject, gameObject);
    }

    [Command]
    private void CmdChangeAttribute(NetworkInstanceId unitId, SkillEnum attEnum, float value)
    {
        if (NetworkServer.objects.ContainsKey(unitId))
            NetworkServer.objects[unitId].GetComponent<Unit>().SetAttribute(attEnum, value);
    }

    [Command]
    public void CmdCreateBuilding(NetworkInstanceId tempBuildingID, BuildingEnum buildingType)
    {
        GameState.Instance.RpcCreateBuilding(tempBuildingID, buildingType);
    }

    [Command]
    private void CmdGather(float amount, NetworkInstanceId resourceId)
    {
        if (NetworkServer.objects.ContainsKey(resourceId))
        {
            Resource resource = NetworkServer.objects[resourceId].GetComponent<Resource>();
            resource.capacity -= amount;
            if (resource.capacity <= 0)
                CmdDestroy(resourceId);
        }
    } 

    [Command]
    private void CmdPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        TemporaryBuilding temporaryBuilding = NetworkServer.objects[tempBuildingId].GetComponent<TemporaryBuilding>();
        temporaryBuilding.transform.position = position;
        temporaryBuilding.placed = true;
        GameState.Instance.RpcPlaceBuilding(position, tempBuildingId);
    }

    [Command]
    private void CmdDestroy(NetworkInstanceId selectableId)
    {
        GameObject selectable = NetworkServer.objects[selectableId].gameObject;
        var bounds = selectable.GetComponent<Collider>().bounds;
        GameState.Instance.RpcDestroyObject(bounds.center, bounds.size);
        NetworkServer.Destroy(selectable);
    }
}