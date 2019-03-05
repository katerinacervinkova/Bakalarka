using UnityEngine;

public class JobBuild : Job {

    private readonly TemporaryBuilding building;
    private readonly Collider buildingCollider;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public JobBuild(TemporaryBuilding building)
    {
        this.building = building;
        buildingCollider = building.GetComponent<Collider>();
    }

    // hledat další stavbu
    public override Job Following => null;

    public override void Do(Unit worker)
    {
        if (Vector3.Distance(buildingCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > 3)
        {
            worker.ResetJob();
            return;
        }
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            building.Build(worker);
            timeElapsed -= minTime;
        }
    }

}
