using UnityEngine;

public class JobLookForTarget : Job
{
    private Job following = null;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public override Job Following => following;

    public override void Do(Unit worker)
    {
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            Selectable target = GameState.Instance.GetNearestTarget(worker.transform.position, 20);
            if (target != null)
            {
                Completed = true;
                following = new JobGo(target.transform.position, new AttackJob(target));
            }
            timeElapsed -= minTime;
        }
    }
}
