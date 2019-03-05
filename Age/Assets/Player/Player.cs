
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public string Name;
    public Color color;
    public List<Unit> units;
    public List<Building> buildings;
    public List<TemporaryBuilding> temporaryBuildings;
    public GameState gameState;


    private void Awake()
    {
        gameState = gameObject.GetComponent<GameState>();
        gameState.player = this;
    }
    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
            gameState.CmdCreateUnit(transform.position, transform.position);
    }

    void Update () {

    }
}