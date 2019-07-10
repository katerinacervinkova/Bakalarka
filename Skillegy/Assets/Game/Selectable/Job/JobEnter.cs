using UnityEngine;

class JobEnter : Job
{
    Building building;
    private Collider buildingCollider;
    public override Job Following { get; }

    public JobEnter(Building building, Job following = null)
    {
        this.building = building;
        Following = following;
        buildingCollider = building.GetComponent<Collider>();
    }
    public override void Do(Unit worker)
    {
        if (!building || Vector3.Distance(buildingCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > 3)
            worker.SetNextJob();
        else if (building.Enter(worker))
        {
            worker.owner.EnterBuilding(worker, building);
            Completed = true;
        }
    }
}
