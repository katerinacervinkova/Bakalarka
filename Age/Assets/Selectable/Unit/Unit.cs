using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;

public class Unit : Selectable
{
    public int Strength { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int Intelligence { get; set; }
    public int Agility { get; set; }
    public int Healing { get; set; }
    public int Crafting { get; set; }
    public int Accuracy { get; set; }

    protected NavMeshAgent navMeshAgent;

    protected override void Awake()
    {
        base.Awake();
        buttons = new List<Button>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    protected override void Update()
    {

    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    public override void RightMouseClickGround(GameObject hitObject, Vector3 hitPoint)
    {
        if (hitObject.name == "Map" && hitPoint != owner.gameWindow.InvalidPosition)
        {
            SetDestination(hitPoint);
        }
        else
        {

        }
    }
    public override void DrawBottomBar()
    {
        nameText.text = Name;
        selectedObjectText.text = string.Format("Health: {0}/{1}", Health, MaxHealth)
        + "\nStrength: " + Strength + "\nIntelligence: " + Intelligence
        + "\nAgility: " + Agility + "\nHealing: " + Healing
        + "\nCrafting: " + Crafting + "\nAccuracy: " + Accuracy;
    }
}