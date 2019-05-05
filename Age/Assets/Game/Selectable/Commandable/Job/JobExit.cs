using UnityEngine;

class JobExit : Job
{
    Building building;
    private Collider buildingCollider;
    public override Job Following { get; }

    public JobExit(Building building, Job following = null)
    {
        this.building = building;
        Following = following;
        buildingCollider = building.GetComponent<Collider>();
    }
    public override void Do(Unit worker)
    {
        building.Exit(worker);
        Completed = true;
    }
}
