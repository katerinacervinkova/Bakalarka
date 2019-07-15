using UnityEngine;

public class JobFollow : Job
{
    private readonly Selectable target;
    private readonly Collider targetCollider;

    private readonly float minTime = 1;
    private float timeElapsed = 1;

    private readonly Job following;

    public JobFollow(Selectable target, Job following)
    {
        this.target = target;
        targetCollider = target.GetComponent<Collider>();
        this.following = following;
    }

    // following job is usually to attack the target
    public override Job Following => following;

    public override void Do(Unit worker)
    {
        // target does not exist
        if (target == null)
        {
            worker.SetNextJob();
            return;
        }

        // repath only once in a while for optimization
        timeElapsed += Time.deltaTime;
        if (timeElapsed > minTime)
        {
            // target is already close enough
            if (Vector3.Distance(targetCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) < worker.Range)
            {
                worker.SetNextJob();
                return;
            }

            // follow the target
            if (target is Unit)
                worker.Go(Vector3.Lerp(target.transform.position, worker.transform.position, 0.5f));
            else
                worker.Go(target.transform.position);

            // repath does not need to be performed multiple times in a frame
            while (timeElapsed > minTime)
                timeElapsed -= minTime;
        }
    }
}
