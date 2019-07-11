using UnityEngine;

public class JobGather<T> : Job where T : Resource {

    private Collider resourceCollider;
    private Vector2 squareID;
    private T resource;
    private readonly float minTime = 1;
    private float timeElapsed = 0;

    private readonly float gatheringIncrease = 0.01f;
    private readonly float slowGatheringIncrease = 0.002f;

    /// <summary>
    /// The following job is to gather from another resource of the same type.
    /// </summary>
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
        // resource does not exist or the unit cannot reach it
        if (resource == null || Vector3.Distance(resourceCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > resource.size.x + 2)
        {
            worker.SetNextJob();
            return;
        }

        // gathering should not happen too often
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            // gather from the resource
            if (resource.Gather(worker.Gathering, worker.owner))
                Completed = true;

            // updates unit's gathering
            if (worker.Gathering < worker.Intelligence)
                worker.owner.ChangeAttribute(worker, SkillEnum.Gathering, worker.Gathering + gatheringIncrease * worker.Intelligence);
            else
                worker.owner.ChangeAttribute(worker, SkillEnum.Gathering, worker.Gathering + slowGatheringIncrease * worker.Intelligence);

            timeElapsed -= minTime;
        }
    }
}
