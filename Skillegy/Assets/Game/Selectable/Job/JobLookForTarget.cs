using UnityEngine;

public class JobLookForTarget : Job
{
    private Job following = null;
    private readonly float minTime = 1;
    private float timeElapsed = 1;
    public override Job Following => following;

    public override void Do(Unit worker)
    {
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            // if any possible target is nearby, attack it
            Selectable target = GameState.Instance.ClosestVisibleTarget(worker.transform.position, worker.playerId);
            if (target != null)
            {
                Completed = true;
                following = new JobFollow(target, new JobAttack(target));
            }
            timeElapsed -= minTime;
        }
    }
}
