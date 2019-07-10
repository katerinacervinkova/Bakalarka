class JobExplore : Job
{
    public override Job Following => new JobGo(GameState.Instance.GetRandomDestination(), this);

    public override void Do(Unit worker) => worker.SetNextJob();
}

