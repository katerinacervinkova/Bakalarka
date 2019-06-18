using UnityEngine;

public class JobAttack : Job {

    private readonly Selectable target;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    private readonly Collider targetCollider;

    public JobAttack(Selectable target)
    {
        this.target = target;
        targetCollider = target.GetComponent<Collider>();
    }

    public override Job Following => new JobLookForTarget(); 

    public override void Do(Unit worker)
    {
        if (target == null)
        {
            worker.SetNextJob();
            return;
        }

        timeElapsed += Time.deltaTime;

        while (timeElapsed > minTime)
        {
            if (Vector3.Distance(targetCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > worker.Range)
            {
                worker.SetJob(new JobFollow(target, this));
                return;
            }
            timeElapsed -= minTime;

            var value = target.Health - worker.Swordsmanship;
            worker.owner.ChangeHealth(target, value);
            if (value <= 0)
                Completed = true;
        }
    }


}
