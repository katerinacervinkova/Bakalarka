public class AttackJob : Job {

    readonly Selectable target;
    public AttackJob(Selectable target)
    {
        this.target = target;
    }

    public override Job Following
    {
        get
        {
            return null;
        }
    }

    public override void Do(Unit worker)
    {

    }


}
