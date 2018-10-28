using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;

public class Unit : Selectable
{
    protected enum UState { Standing, Moving, MovingClose, MovingVeryClose }


    protected UState state;
    protected float closeDistance = 5;
    protected float veryCloseDistance = 1;
    public Regiment regiment { get; set; }
    public int Strength { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int Intelligence { get; set; }
    public int Agility { get; set; }
    public int Healing { get; set; }
    public int Crafting { get; set; }
    public int Accuracy { get; set; }

    public NavMeshAgent navMeshAgent { get; set; }

    public bool IsStanding { get { return state == UState.Standing; } }
    protected override void Awake()
    {
        base.Awake();
        buttons = new List<Button>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 0;
        gridGraph.AddSelectable(this);
    }

    protected override void Start()
    {
    }

    protected override void Update()
    {
        if (state == UState.Moving && navMeshAgent.remainingDistance < closeDistance)
            SetExactDestination();

        else if (state != UState.Standing && navMeshAgent.remainingDistance < veryCloseDistance)
        {
            gridGraph.AddSelectable(this);
            state = UState.Standing;
        }
        else if (state == UState.MovingClose)
        {
            veryCloseDistance += Time.deltaTime;
            if (veryCloseDistance > 1)
            {
                veryCloseDistance -= 1;
                gridGraph.SetDestination(navMeshAgent.destination, navMeshAgent);
            }
        }
    }

    public void SetExactDestination()
    {
        gridGraph.SetDestination(navMeshAgent.destination, navMeshAgent);
        state = UState.MovingClose;
    }

    public void SetExactDestination(Vector3 destination)
    {
        gridGraph.SetDestination(destination, navMeshAgent);
        state = UState.MovingClose;
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
        state = UState.Moving;
    }

    public override void RightMouseClickGround(GameObject hitObject, Vector3 hitPoint)
    {
        if (hitObject.name == "Map" && hitPoint != owner.gameWindow.InvalidPosition)
        {
            if (regiment != null)
            {
                regiment.Remove(this);
                regiment = null;
            }
            if (state == UState.Standing)
                gridGraph.RemoveSelectable(this);
            if (Vector3.Distance(transform.position, hitPoint) < closeDistance)
                SetExactDestination(hitPoint);
            else
                SetDestination(hitPoint);
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