/// <summary>
/// The unit travels randomly, when it reaches its destination, it is given a new one
/// </summary>
class JobExplore : Job
{
    // go to the random destination and then return to this job
    public override Job Following => new JobGo(GameState.Instance.GetRandomDestination(), this);

    // destination has been reached, go to the next one
    public override void Do(Unit worker) => worker.SetNextJob();
}

