using UnityEngine;

public class JobBuild : Job {

    private readonly int playerId;
    private readonly TemporaryBuilding building;
    private readonly Vector3 buildingPos;
    private readonly Collider buildingCollider;
    private readonly float minTime = 1;
    private float timeElapsed = 0;

    private readonly float buildingIncrease = 0.005f;
    private readonly float slowBuildingIncrease = 0.001f;

    public JobBuild(TemporaryBuilding building, int playerId)
    {
        this.building = building;
        buildingPos = building.transform.position;
        buildingCollider = building.GetComponent<Collider>();
        this.playerId = playerId;
    }

    /// <summary>
    /// the next job is to build another temporary building
    /// </summary>
    public override Job Following
    {
        get
        {
            TemporaryBuilding tempBuilding = PlayerState.Get(playerId).GetNearestTempBuilding(building, buildingPos, 20);
            if (tempBuilding == null)
                return null;
            return new JobGo(tempBuilding.FrontPosition, tempBuilding.GetOwnJob(null));
        }
    }


    public override void Do(Unit worker)
    {
        // building doesn't exist or the builder cannot reach it
        if (!building || Vector3.Distance(buildingCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > 3)
        {
            worker.SetNextJob();
            return;
        }
        
        // building cannot happen too often
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            building.Build(worker.Building);

            // increases the builder's working skill
            if (worker.Building < worker.Intelligence)
                worker.owner.ChangeAttribute(worker, SkillEnum.Building, worker.Building + buildingIncrease * worker.Intelligence);
            else
                worker.owner.ChangeAttribute(worker, SkillEnum.Building, worker.Building + slowBuildingIncrease * worker.Intelligence);
            timeElapsed -= minTime;
        }
    }

}
