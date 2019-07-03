using UnityEngine;

public class JobGather<T> : Job where T : Resource {

    private Collider resourceCollider;
    private Vector2 squareID;
    private T resource;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public override Job Following
    {
        get
        {
            T res = GameState.Instance.GetClosestResource(squareID, resource);
            if (res == null)
                return null;
            return new JobGo(res.FrontPosition, res.GetOwnJob(null));
        }
    }

    public JobGather(T resource)
    {
        this.resource = resource;
        resourceCollider = resource.GetComponent<Collider>();
        squareID = resource.SquareID;
    }

    public override void Do(Unit worker)
    {
        if (resource == null || Vector3.Distance(resourceCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > resource.size.x + 2)
        {
            worker.SetNextJob();
            return;
        }
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            if (resource.Gather(worker.Gathering, worker.owner))
                Completed = true;
            worker.owner.ChangeAttribute(worker, AttEnum.Gathering, worker.Gathering + 0.1f);
            timeElapsed -= minTime;
        }
    }
}
