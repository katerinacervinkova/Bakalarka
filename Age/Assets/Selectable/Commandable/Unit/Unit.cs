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
    public int MaxHealth { get; set; }
    public int Health { get; set; }
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
        base.Update();
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


    public override void SetSelection(bool selected, Player player)
    {
        base.SetSelection(selected, player);
        bottomBar.SetActive(this, selected);
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