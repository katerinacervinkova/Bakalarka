using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : Selectable {

    public Vector3 SpawnPoint { get; private set; }
    public Vector3 DefaultDestination { get; private set; }

    bool animating = false;
    protected List<Scheduler> schedulers = new List<Scheduler>();

    public override void OnStartClient() { }
    public override void Init()
    {
        base.Init();
        minimapColor = owner.color;
        minimapIcon.color = minimapColor; 
        DefaultDestination = SpawnPoint = transform.position;
        DrawHealthBar();
        transform.Find("Floor1").GetComponent<MeshRenderer>().material.color = owner.color;
        transform.Find("Floor2").GetComponent<MeshRenderer>().material.color = owner.color;
        if (hasAuthority)
        {
            playerState.buildings.Add(this);
            InitTransactions();
        }
    }

    protected override void Update()
    {
        if (animating && schedulers.Count == 0)
            animating = false;
        base.Update();
    }
    public override void RightMouseClickGround(Vector3 hitPoint)
    {
        DefaultDestination = hitPoint;
    }

    public void AddScheduler(Scheduler scheduler)
    {
        scheduler.gameObject.transform.Translate(50 * schedulers.Count, 0, 0);
        scheduler.gameObject.SetActive(true);
        scheduler.schedulers = schedulers;
        schedulers.Add(scheduler);
        if (!animating)
        {
            scheduler.Animate();
            animating = true;
        }
        
    }

    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        nameText.text = Name;
        selectedObjectText.text = "";
        SetSchedulersActive(true);
    }

    public override void RemoveBottomBar(Text nameText, Text selectedObjectText)
    {
        SetSchedulersActive(false);
    }

    protected void SetSchedulersActive(bool active)
    {
        schedulers.ForEach(scheduler =>
        {
            CanvasGroup canvasGroup = scheduler.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = active;
            canvasGroup.alpha = active ? 1 : 0;

        });
    }

    protected override Job GetEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar(Health / (float)MaxHealth);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        playerState.buildings.Remove(this);
    }
}