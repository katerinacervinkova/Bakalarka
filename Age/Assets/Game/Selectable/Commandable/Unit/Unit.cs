using Pathfinding;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Unit : Commandable
{
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

    private Job job;

    protected AIPath aiPath;
    public MovementController movementController;

    internal void ResetJob()
    {
        job = null;
    }

    protected void Awake()
    {
        aiPath = GetComponent<AIPath>();
        movementController = GetComponent<MovementController>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        minimapColor = owner.color;
        minimapIcon.color = minimapColor;
        transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = owner.color;
    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        PlayerState.Instance.units.Add(this);
    }
    protected override void Update()
    {
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



    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        if (!hasAuthority)
            return;
        job = new JobGo(hitPoint);
    }

    public override string GetObjectDescription()
    {
        if (hasAuthority)
            return string.Format("Health: {0}/{1}", Health, MaxHealth)
            + "\nStrength: " + Strength + "\nIntelligence: " + Intelligence
            + "\nAgility: " + Agility + "\nHealing: " + Healing
            + "\nCrafting: " + Crafting + "\nAccuracy: " + Accuracy;
        return string.Format("Health: {0}/{1}", Health, MaxHealth);
    }

   
    public override void DrawHealthBar()
    {
        DrawProgressBar(Health / (float)MaxHealth);
    }

    public override void SetGoal(Selectable goal)
    {
        Job following = goal.CreateJob(this);
        job = new JobGo(goal.transform.position, following);
    }

    public void SetJob(Job job)
    {
        this.job = job;
    }

    public void Go(Vector3 destination)
    {
        aiPath.destination = destination;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerState.Instance?.units.Remove(this);
    }

    public void OnTargetReached()
    {
        if (job is JobGo)
            job.Completed = true;
        //GameState.Instance.SetPoint(this, transform.position);
    }
}