using Age;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int startFood, startWood, startCoin;
    public string Name;
    public bool IsHuman;
    public Color color;
    public Selectable SelectedObject { get; set; }
    public Selectable[] units;
    public PlayerInputOptions inputOptions;
    public Factory factory;
    public Dictionary<Globals.ResourceType, int> Resources { get; private set; }


    void Start () {
        inputOptions = gameObject.GetComponent<PlayerInputOptions>();
    }
	

	void Update () {

    }

    private void AddResource(Globals.ResourceType resourceType, int amount)
    {
        Resources[resourceType] += amount;
    }
}