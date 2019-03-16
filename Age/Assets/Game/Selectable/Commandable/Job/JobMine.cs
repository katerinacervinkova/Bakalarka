using UnityEngine;

public class JobMine : Job {

    private Resource resource;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    private Collider resourceCollider;
    public override Job Following
    {
        get
        {
            return null;
        }
    }

    public JobMine(Resource resource)
    {
        this.resource = resource;
        resourceCollider = resource.GetComponent<Collider>();
    }

    public override void Do(Unit worker)
    {
        if (!resource || Vector3.Distance(resourceCollider.ClosestPointOnBounds(worker.transform.position), worker.transform.position) > 3)
        {
            worker.ResetJob();
            return;
        }
        timeElapsed += Time.deltaTime;
        while (timeElapsed > minTime)
        {
            resource.Mine(worker);
            timeElapsed -= minTime;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
