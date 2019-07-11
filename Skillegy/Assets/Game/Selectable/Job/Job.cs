/// <summary>
/// Base class for all units' task.
/// </summary>
public abstract class Job {

    public bool Completed = false;
    /// <summary>
    /// Job that the unit should take after this one.
    /// </summary>
    public abstract Job Following { get; }
    /// <summary>
    /// Called every frame from all units that are now performing this job.
    /// </summary>
    /// <param name="worker">unit that wants to perform the job</param>
    public abstract void Do(Unit worker);
}