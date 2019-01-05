public abstract class Job {

    public bool Completed = false;
    public abstract Job Following { get; }
    public abstract void Do(Unit worker);
}
