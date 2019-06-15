using Pathfinding;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    public override void OnStartLocalPlayer()
    {
        PlayerState.Set(playerControllerId, factory.CreatePlayerState());
        PlayerState.Get(playerControllerId).playerPurchases = factory.CreatePlayerPurchases();
        if (IsHuman)
            Camera.main.transform.parent.position = transform.position;
    }

    public bool Init()
    {
        if (!hasAuthority || (connectionToClient != null && !connectionToClient.isReady) || GameState.Instance == null)
            return false;
        GameState.Instance.SetVisibilitySquares(playerControllerId, factory.CreateVisibilitySquares());

        CmdCreateUnit(transform.position, transform.position);
        return true;
    }

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
            endGameCanvas.SetActive(false);
        }
        CmdChangeInGame(true);
    }

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

    public void Lose()
    {
        if (IsHuman)
            endGameCanvas.GetComponent<Text>().text = "You lose!";
        EndGame();
    }

    public void Win()
    {
        if (IsHuman)
            endGameCanvas.GetComponent<Text>().text = "You win!";
        EndGame();
    }

    public void EndGame()
    {
        CmdChangeInGame(false);
        if (IsHuman)
        {
            endGameCanvas.SetActive(true);
            endGameCanvas.transform.Find("MenuButton").GetComponent<Button>().onClick.AddListener(OnClickMainMenu);
            ((HumanVisibilitySquares)GameState.Instance.GetSquares(playerControllerId)).SeeEverything();
            Destroy(GameObject.Find("MainCanvas"));
        }
    }

    private void OnClickMainMenu()
    {

    }

    public void ExitBuilding(Unit unit, Building building)
    {
        if (unit != null && building != null)
            CmdExitBuilding(unit.netId, building.FrontPosition, building.DefaultDestination);
    }

    public void EnterBuilding(Unit unit, Building building)
    {
        CmdEnterBuilding(unit.netId);
    }

    public void Gather(float amount, Resource resource)
    {
        CmdGather(amount, resource.netId);
    }

    public void ChangeAttribute(Unit unit, AttEnum attEnum, float value)
    {
        CmdChangeAttribute(unit.netId, attEnum, value);
    }

    public void CreateTempBuilding(BuildingEnum buildingType)
    {
        CmdCreateTempBuilding(buildingType);
    }

    public void CreateUnit(Building building)
    {
        CmdCreateUnit(building.FrontPosition, building.DefaultDestination);
    }

    public void PlaceBuilding(TemporaryBuilding temporaryBuilding)
    {
        CmdPlaceBuilding(temporaryBuilding.transform.position, temporaryBuilding.netId);
    }

    public void ChangeHealth(Selectable selectable, float value)
    {
        CmdChangeHealth(selectable.netId, value);
    }

    public void DestroySelectedObject(Selectable selectedObject)
    {
        CmdDestroy(selectedObject.netId);
    }

    private Vector3 NearestWalkable(Vector3 position)
    {
        NNConstraint nodeConstraint = new NNConstraint
        {
            constrainWalkability = true,
            walkable = true
        };
        return AstarPath.active.GetNearest(position, nodeConstraint).position;
    }

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
    public void CmdCreateTempBuilding(BuildingEnum buildingType)
    {
        var tempBuilding = factory.CreateTemporaryMainBuilding(netId, buildingType);
        NetworkServer.SpawnWithClientAuthority(tempBuilding.gameObject, gameObject);
    }

    [Command]
    private void CmdChangeAttribute(NetworkInstanceId unitId, AttEnum attEnum, float value)
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
    public void CmdGather(float amount, NetworkInstanceId resourceId)
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
    public void CmdPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        TemporaryBuilding temporaryBuilding = NetworkServer.objects[tempBuildingId].GetComponent<TemporaryBuilding>();
        temporaryBuilding.transform.position = position;
        temporaryBuilding.placed = true;
        GameState.Instance.RpcPlaceBuilding(position, tempBuildingId);
    }

    [Command]
    public void CmdDestroy(NetworkInstanceId selectableId)
    {
        GameObject selectable = NetworkServer.objects[selectableId].gameObject;
        var bounds = selectable.GetComponent<Collider>().bounds;
        GameState.Instance.RpcDestroyObject(bounds.center, bounds.size);
        NetworkServer.Destroy(selectable);
    }
}