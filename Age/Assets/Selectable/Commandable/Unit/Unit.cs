using UnityEngine;
using UnityEngine.AI;

public class Unit : Commandable
{
    protected enum UState { Standing, Moving, MovingClose, MovingVeryClose }


    protected UState state;
    protected float closeDistance = 5;
    protected float veryCloseDistance = 1;
    public Regiment Reg { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Agility { get; set; }
    public int Healing { get; set; }
    public int Crafting { get; set; }
    public int Accuracy { get; set; }

    protected NavMeshAgent Agent { get; set; }

    public override bool Arrived => state == UState.Standing;

    protected override void Awake()
    {
        base.Awake();
        Agent = gameObject.GetComponent<NavMeshAgent>();
        Agent.stoppingDistance = 0;
        gridGraph.AddSelectable(this);
    }

    protected override void Start()
    {
    }

    protected override void Update()
    {
        Move();
        JobUpdate();
        base.Update();
    }

    protected virtual void JobUpdate()
    {
        if (job == null)
            return;
        if (job.Completed)
            job = job.Following;
        job?.Do(this);
    }

    protected void Move()
    {
        if (state == UState.Moving && Agent.remainingDistance < closeDistance)
            SetExactDestination();

        else if (state != UState.Standing && Agent.remainingDistance < veryCloseDistance)
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
                gridGraph.SetDestination(Agent.destination, Agent);
            }
        }
    }

    private void SetExactDestination()
    {
        gridGraph.SetDestination(Agent.destination, Agent);
        state = UState.MovingClose;
    }

    private void SetExactDestination(Vector3 destination)
    {
        gridGraph.SetDestination(destination, Agent);
        state = UState.MovingClose;
    }

    public override void SetDestination(Vector3 destination)
    {
        Agent.SetDestination(destination);
        state = UState.Moving;
    }

    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        if (hitPoint != owner.gameWindow.InvalidPosition)
        {
            Reg?.Remove(this);
            Reg = null;
            if (state == UState.Standing)
                gridGraph.RemoveSelectable(this);
            job = new JobGo(this, hitPoint);
        }
    }

    public override void RightMouseClickObject(Selectable hitObject)
    {
        SetGoal(hitObject);
    }
    protected override void DrawNameText()
    {
        nameText.text = Name;
    }
    protected override void DrawSelectedObjectText()
    {
        selectedObjectText.text = string.Format("Health: {0}/{1}", Health, MaxHealth)
        + "\nStrength: " + Strength + "\nIntelligence: " + Intelligence
        + "\nAgility: " + Agility + "\nHealing: " + Healing
        + "\nCrafting: " + Crafting + "\nAccuracy: " + Accuracy;
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

    protected override void SetEvents()
    {

    }

    protected override void RemoveEvents()
    {
    }
}