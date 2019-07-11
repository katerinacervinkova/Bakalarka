using UnityEngine;

public class JobGo : Job
{
    private Unit worker;
    private Vector3 destination;
    private readonly Job following;

    public JobGo(Vector3 destination, Job following = null)
    {
        this.following = following;
        this.destination = destination;
    }

    public override Job Following => following;

    public override void Do(Unit unit)
    {
        // the actual function within the if statement is called only for the first time
        if (worker != unit)
        {
            worker = unit;
            worker.Go(destination);
        }
    }
}
