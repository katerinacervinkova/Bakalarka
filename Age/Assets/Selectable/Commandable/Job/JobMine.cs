using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobMine : Job {

    private Resource resource;
    private readonly float minTime = 1;
    private float timeElapsed = 0;
    public override Job Following
    {
        get
        {
            return null;
        }
    }

    public JobMine(Resource resource) { this.resource = resource; }

    public override void Do(Unit worker)
    {
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
