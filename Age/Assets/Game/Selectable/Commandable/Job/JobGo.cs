using UnityEngine;

public class JobGo : Job
{
    private readonly Job following;
    private Unit worker;
    private Vector3 destination;

    public JobGo(Vector3 destination, Job following = null)
    {
        this.following = following;
        this.destination = destination;
    }

    public override Job Following => following;

    public override void Do(Unit unit)
    {
        if (worker != unit)
        {
            worker = unit;
            worker.Go(destination);
        }
    }
}
