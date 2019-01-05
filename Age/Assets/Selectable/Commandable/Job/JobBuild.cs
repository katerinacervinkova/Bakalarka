using UnityEngine;

public class JobBuild : Job {

    private readonly TemporaryBuilding building;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public JobBuild(TemporaryBuilding building)
    {
        this.building = building;
    }

    // hledat další stavbu
    public override Job Following => null;

    public override void Do(Unit worker)
    {
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            building.Build(worker);
            timeElapsed -= minTime;
        }
    }

}
