using UnityEngine;

public class JobGather<T> : Job where T : Resource {

    private Vector3 resourcePosition;
    private T resource;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public override Job Following
    {
        get
        {
            T res = GameState.Instance.GetNearestResource(resource, resourcePosition, 20);
            if (res == null)
                return null;
            return new JobGo(res.transform.position, res.GetOwnJob(null));
        }
    }

    public JobGather(T resource)
    {
        this.resource = resource;
        resourcePosition = resource.transform.position;
    }

    public override void Do(Unit worker)
    {
        if (!resource || Vector3.Distance(resourcePosition, worker.transform.position) > resource.size + 3)
        {
            worker.ResetJob();
            return;
        }
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            resource.Gather(worker.Gathering, worker.owner);
            worker.owner.ChangeAttribute(worker, AttEnum.Gathering, worker.Gathering + 0.1f);
            timeElapsed -= minTime;
        }
    }
}
