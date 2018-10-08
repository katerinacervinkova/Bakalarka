using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable {

    protected Vector3 spawnPoint;
    protected Vector3 defaultDestination;
    public Factory factory;

    bool animating = false;
    protected List<Scheduler> schedulers = new List<Scheduler>();

    protected override void Awake()
    {
        base.Awake();
        SetSpawnPoint();
        SetDefaultButtonEvents();
    }

    private void SetDefaultButtonEvents()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].onClick.AddListener(CreateUnit);
        }
    }

    protected override void Update()
    {
        if (animating && schedulers.Count == 0)
            animating = false;
        base.Update();
    }
    public override void RightMouseClickGround(GameObject hitObject, Vector3 hitPoint)
    {
        defaultDestination = hitPoint;
    }

    private void SetSpawnPoint()
    {
        Bounds bounds = gameObject.GetComponent<BoxCollider>().bounds;
        Vector3 ext = bounds.extents;
        ext.x = -ext.x - 1;
        ext.z = ext.z + 1;
        spawnPoint = bounds.center - ext;
        defaultDestination = spawnPoint;
    }

    public void CreateUnit()
    {
        Scheduler scheduler = factory.CreateScheduler(schedulers, () => factory.CreateUnit(owner, spawnPoint, defaultDestination), null);
        schedulers.Add(scheduler);
        if (!animating)
        {
            scheduler.Animate();
            animating = true;
        }
    }
    public override void DrawBottomBar()
    {
        nameText.text = "Building";
        selectedObjectText.text = "";
    }
}