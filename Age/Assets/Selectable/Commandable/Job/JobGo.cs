using UnityEngine;

public class JobGo : Job
{
    private readonly Job following;

    public JobGo(Unit worker, Vector3 destination, Job following = null)
    {
        this.worker = worker;
        this.following = following;
        worker.SetDestination(destination);
    }

    public override Job Following => following;

    public override void Do()
    {
        if (worker.Arrived)
            Completed = true;
    }
}
