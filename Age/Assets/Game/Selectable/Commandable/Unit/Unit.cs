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

    private Job job;

    protected NavMeshAgent Agent { get; set; }

    private bool pending = false;

    [SyncVar]
    public bool Arrived;

    internal void ResetJob()
    {
        job = null;
    }

    protected void Awake()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();
        desiredLocation = steeringLocation = transform.position;
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
        playerState.units.Add(this);
    }
    protected override void Update()
    {
        Move();
        Debug.DrawRay(steeringLocation, Vector3.up * 10, Color.blue);
        Debug.DrawRay(desiredLocation, Vector3.up * 15, Color.green);
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
        if (!hasAuthority || Arrived || Agent.pathPending)
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
            {
                transform.position = steeringLocation;
                owner.CmdUnitArrived(true, netId);
            }
        }
        else if (gameState.IsOccupied(Agent.steeringTarget))
            Repath();
        
    }
    private bool AlmostThere()
    {
        return Vector3.Distance(transform.position, steeringLocation) < 0.5;
    }

    private void Repath()
    {
        steeringLocation = gameState.GetClosestUnoccupiedDestination(desiredLocation);
        if (steeringLocation != Agent.pathEndPosition)
            SyncDestination(steeringLocation);
    }

    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        if (!hasAuthority)
            return;
        job = new JobGo(this, hitPoint);
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
        job = new JobGo(this, goal.transform.position, following);
    }

    public void SetJob(Job job)
    {
        this.job = job;
    }

    public void Go(Vector3 destination)
    {
        if (!hasAuthority)
            return;
        Arrived = false;
        owner.CmdUnitArrived(false, netId);
        desiredLocation = gameState.GetClosestDestination(destination);
        steeringLocation = desiredLocation;
        SyncDestination(steeringLocation);
        pending = true;
    }

    public void SetDestination(Vector3 destination)
    {
        steeringLocation = destination;
        Agent.SetDestination(steeringLocation);
    }

    private void SyncDestination(Vector3 destination)
    {
        owner.CmdSetDestination(destination, netId);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        playerState.units.Remove(this);
    }
}