using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Unit : Commandable
{
    protected enum UState { Standing, Moving, MovingClose, MovingVeryClose }

    Vector3 desiredLocation;
    Vector3 steeringLocation;
    public Regiment Reg { get; set; }
    [SyncVar]
    public int Strength;
    [SyncVar]
    public int Intelligence;
    [SyncVar]
    public int Agility;
    [SyncVar]
    public int Healing;
    [SyncVar]
    public int Crafting;
    [SyncVar]
    public int Accuracy;

    protected NavMeshAgent agent { get; set; }

    private bool pending = false;

    [SyncVar(hook = "OnArrivedChange")]
    private bool arrived;
    public override bool Arrived => arrived;

    internal void ResetJob()
    {
        job = null;
    }

    protected void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        desiredLocation = steeringLocation = transform.position;
    }

    protected override void Update()
    {
        Move();
        JobUpdate();
        base.Update();
    }

    protected virtual void JobUpdate()
    {
        if (!hasAuthority)
            return;
        if (job != null && job.Completed)
            job = job.Following;
        job?.Do(this);
    }


    protected void Move()
    {
        if (!hasAuthority || arrived || agent.pathPending)
            return;
        if (pending)
        {
            pending = false;
            Repath();
        }
        if (AlmostThere())
        {
            if (steeringLocation != desiredLocation)
                Repath();
            if (AlmostThere())
                CmdChangeArrived(true);
        }
        else if (gameState.IsOccupied(agent.steeringTarget))
            Repath();
        
    }
    private bool AlmostThere()
    {
        return Vector3.Distance(transform.position, steeringLocation) < 1;
    }

    [Command]
    public void CmdChangeArrived(bool value)
    {
        arrived = value;
    }

    private void OnArrivedChange(bool value)
    {
        if (value)
            gameState.AddSelectable(this);
        else
            gameState.RemoveSelectable(this);
        arrived = value;
    }
    private void Repath()
    {
        steeringLocation = gameState.GetClosestUnoccupiedDestination(desiredLocation);
        if (steeringLocation != agent.pathEndPosition)
            agent.SetDestination(steeringLocation);
    }

    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        if (!hasAuthority)
            return;
        Reg?.Remove(this);
        Reg = null;
        job = new JobGo(this, hitPoint);
    }

    public override void RightMouseClickObject(Selectable hitObject)
    {
        SetGoal(hitObject);
    }
    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        nameText.text = Name;
        if (hasAuthority)
            selectedObjectText.text = string.Format("Health: {0}/{1}", Health, MaxHealth)
            + "\nStrength: " + Strength + "\nIntelligence: " + Intelligence
            + "\nAgility: " + Agility + "\nHealing: " + Healing
            + "\nCrafting: " + Crafting + "\nAccuracy: " + Accuracy;
        else
            selectedObjectText.text = string.Format("Health: {0}/{1}", Health, MaxHealth);
    }

   
    public override void DrawHealthBar()
    {
        DrawProgressBar(Health / (float)MaxHealth);
    }

    public override void SetGoal(Selectable goal)
    {
        Job following = goal.CreateJob(this);
        job = new JobGo(this as Unit, goal.transform.position, following);
    }

    public void SetJob(Job job)
    {
        this.job = job;
    }

    public override void SetDestination(Vector3 destination)
    {
        arrived = false;
        CmdChangeArrived(false);
        desiredLocation = gameState.GetClosestDestination(destination);
        steeringLocation = desiredLocation;
        agent.SetDestination(steeringLocation);
        pending = true;
    }
}