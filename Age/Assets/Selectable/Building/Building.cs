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
        spawnPoint = bounds.center;
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

    protected override void BottomBarUI(bool selected, Player player)
    {
        base.BottomBarUI(selected, player);
        schedulers.ForEach(scheduler => 
        {
            CanvasGroup canvasGroup = scheduler.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = selected;
            canvasGroup.alpha = selected ? 1 : 0;
            
        });
    }
}