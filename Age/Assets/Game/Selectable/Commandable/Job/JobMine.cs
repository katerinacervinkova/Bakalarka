using UnityEngine;

public class JobMine<T> : Job where T : Resource {

    private Vector3 resourcePosition;
    private int resourceSize;
    private T resource;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public override Job Following
    {
        get
        {
            T res = GameState.Instance.GetNearestResource(resource, resourcePosition, 20);
            if (res == null)
                return null;
            return new JobGo(res.transform.position, res.GetOwnJob(null));
        }
    }

    public JobMine(T resource)
    {
        this.resource = resource;
        resourceSize = resource.size;
        resourcePosition = resource.transform.position;
    }

    public override void Do(Unit worker)
    {
        if (!resource || Vector3.Distance(resourcePosition, worker.transform.position) > resourceSize + 3)
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
