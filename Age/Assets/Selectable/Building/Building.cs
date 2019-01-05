using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable {

    public Vector3 spawnPoint;
    protected Vector3 defaultDestination;

    bool animating = false;
    protected List<Scheduler> schedulers = new List<Scheduler>();
    protected override void Start()
    {
        base.Start();
        defaultDestination = spawnPoint = transform.position;
        DrawHealthBar();
    }
    protected override void Update()
    {
        if (animating && schedulers.Count == 0)
            animating = false;
        base.Update();
    }
    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        defaultDestination = hitPoint;
    }

    public void CreateUnit()
    {
        Scheduler scheduler = owner.factory.CreateScheduler(schedulers, () => owner.factory.CreateUnit(owner, spawnPoint, defaultDestination), null);
        schedulers.Add(scheduler);
        if (!animating)
        { 
            scheduler.Animate();
            animating = true;
        }
    }
    protected override void DrawNameText()
    {
        nameText.text = Name;
    }
    protected override void DrawSelectedObjectText()
    {
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

    protected override Job CreateOwnJob(Commandable worker)
    {
        throw new System.NotImplementedException();
    }

    protected override Job CreateEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar(Health / (float)MaxHealth);
    }

    protected override void SetEvents()
    {
        
    }

    protected override void RemoveEvents()
    {
        
    }
}