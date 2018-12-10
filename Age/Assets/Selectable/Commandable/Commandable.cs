using UnityEngine;

public abstract class Commandable : Selectable {

    protected Job job;
    public abstract void SetDestination(Vector3 destination);
    public abstract bool Arrived { get; }

    public void SetGoal(Selectable goal)
    {
        Job following = goal.CreateJob(this);
        job = new JobGo(this as Unit, goal.transform.position, following);
    }

    public void SetGo(Vector3 destination)
    {
        job = new JobGo(this as Unit, destination);
    }

    protected override void Update()
    {
        JobUpdate();
    }

    protected void JobUpdate()
    {
        if (job == null)
            return;
        if (job.Completed)
            job = job.Following;
        job?.Do();
    }

    protected override Job CreateOwnJob(Commandable worker)
    {
        return null;
    }
    protected override Job CreateEnemyJob(Commandable worker)
    {
        return new AttackJob(worker, this);
    }
}
