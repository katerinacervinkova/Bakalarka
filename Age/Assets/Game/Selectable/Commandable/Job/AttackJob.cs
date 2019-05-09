using UnityEngine;

public class AttackJob : Job {

    readonly Selectable target;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    private readonly Collider targetCollider;

    public AttackJob(Selectable target)
    {
        this.target = target;
        targetCollider = target.GetComponent<Collider>();
    }

    public override Job Following => new JobLookForTarget(); 

    public override void Do(Unit worker)
    {
        if (!target || Vector3.Distance(targetCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > 5)
        {
            worker.SetNextJob();
            return;
        }
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            var value = target.Health - worker.Swordsmanship;
            worker.owner.ChangeHealth(target, value);
            if (value <= 0)
                Completed = true;
            timeElapsed -= minTime;
        }
    }


}
