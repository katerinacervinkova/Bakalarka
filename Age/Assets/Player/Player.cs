
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string Name;
    public bool IsHuman;
    public Color color;
    public Selectable SelectedObject { get; set; }
    public List<Selectable> units;
    public PlayerInputOptions inputOptions;
    public Factory factory;
    public GameWindow gameWindow;

    void Start () {
        inputOptions = gameObject.GetComponent<PlayerInputOptions>();
        gameWindow = gameObject.GetComponent<GameWindow>();
        foreach (Unit unit in units)
            unit.owner = this;
    }
	

	void Update () {

    }
}