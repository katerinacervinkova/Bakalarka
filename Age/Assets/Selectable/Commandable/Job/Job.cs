public abstract class Job {

    public Unit worker;
    public bool Completed = false;
    public abstract Job Following { get; }
    public abstract void Do();
}
