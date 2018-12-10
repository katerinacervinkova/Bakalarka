using UnityEngine;

public class JobBuild : Job {

    private readonly TemporaryBuilding building;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public JobBuild(Unit worker, TemporaryBuilding building)
    {
        this.worker = worker;
        this.building = building;
    }

    // hledat další stavbu
    public override Job Following => null;

    public override void Do()
    {
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            building.Build(this);
            timeElapsed -= minTime;
        }
    }

}
