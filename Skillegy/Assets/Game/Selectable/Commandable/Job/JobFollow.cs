using UnityEngine;

public class JobFollow : Job
{
    private readonly Selectable target;
    private readonly float minTime = 1;
    private float timeElapsed = 1;
    private readonly Collider targetCollider;
    private readonly Job following;

    public JobFollow(Selectable target, Job following)
    {
        this.target = target;
        targetCollider = target.GetComponent<Collider>();
        this.following = following;
    }

    public override Job Following => following;

    public override void Do(Unit worker)
    {
        if (target == null)
        {
            worker.SetNextJob();
            return;
        }

        timeElapsed += Time.deltaTime;

        if (timeElapsed > minTime)
        {
            if (Vector3.Distance(targetCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) < worker.Range)
            {
                worker.SetNextJob();
                return;
            }
            worker.Go(target.transform.position);
            while (timeElapsed > minTime)
                timeElapsed -= minTime;
        }
    }
}
