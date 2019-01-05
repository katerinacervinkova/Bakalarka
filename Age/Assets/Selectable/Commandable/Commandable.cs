using UnityEngine;

public abstract class Commandable : Selectable {

    protected Job job;
    public abstract void SetDestination(Vector3 destination);
    public abstract bool Arrived { get; }

    public abstract void SetGoal(Selectable goal);

    public void SetGo(Vector3 destination)
    {
        job = new JobGo(this as Unit, destination);
    }
 
    protected override Job CreateOwnJob(Commandable worker)
    {
        return null;
    }
    protected override Job CreateEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }
}
