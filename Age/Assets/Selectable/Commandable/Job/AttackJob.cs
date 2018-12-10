public class AttackJob : Job {

    readonly Selectable target;
    public AttackJob(Commandable worker, Selectable target)
    {
        this.worker = worker as Unit;
        this.target = target;
    }

    public override Job Following
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public override void Do()
    {
        throw new System.NotImplementedException();
    }


}
