using UnityEngine;

public class JobAttack : Job {

    private readonly Selectable target;
    private readonly Collider targetCollider;

    private readonly float minTime = 1;
    private float timeElapsed = 0;

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

        // attacking cannot happen too often
        while (timeElapsed > minTime)
        {
            // target it too far away, follow him
            if (Vector3.Distance(targetCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > worker.Range)
            {
                worker.SetJob(new JobFollow(target, this));
                return;
            }

            // substracts the worker swordmanship from target's health
            var value = target.Health - worker.Swordsmanship;
            worker.owner.ChangeHealth(target, value);

            // informs him that he's being attacked
            worker.owner.Attack(worker, target);

            // job is completed when the target is dead
            if (value <= 0)
                Completed = true;

            timeElapsed -= minTime;

        }
    }


}
