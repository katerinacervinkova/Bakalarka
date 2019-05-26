using System;
using UnityEngine;
using UnityEngine.Networking;

public class TemporaryBuilding : Selectable
{
    public BuildingEnum buildingType;
    static readonly int maxProgress = 100;
    [SyncVar]
    public bool placed = false;

    private Job buildJob = null;
    [SyncVar(hook = "OnProgressChange")]
    private float progress = 0;
    private Collider coll;

    public override string Name => buildingType.ToString();
    public override float HealthValue => progress / maxProgress;
    public Bounds Bounds => coll.bounds;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        PlayerState.Instance.SetTempBuilding(this);
        PlayerState.Instance.temporaryBuildings.Add(this);
    }

    public override void Init()
    {
        base.Init();
        minimapColor = owner.color;
        minimapIcon.color = minimapColor;
        GameState.Instance.TemporaryBuildings.Add(this);
        transform.Find("Image").GetComponent<SpriteRenderer>().color = owner.color;
        coll = GetComponent<Collider>();
    }

    private void OnProgressChange(float newProgress)
    {
        progress = newProgress;
        PlayerState.Instance?.OnStateChange(this);
        if (initialized && PlayerState.Instance?.SelectedObject != this)
            healthBar.HideAfter();
    }

    public void OnPlaced(Vector3 position)
    {
        transform.position = position;
        GetComponent<Collider>().enabled = true;
        owner.PositionChange(this);
        transform.Find("Building").gameObject.SetActive(false);
        transform.Find("Fence").gameObject.SetActive(true);
        transform.Find("Image").gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcOnCreate()
    {
        if (hasAuthority)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    [Command]
    private void CmdBuild(float strength)
    {
        progress += Math.Min(maxProgress - progress, strength);
    }
    public void Build(float building)
    {
        if (!hasAuthority)
            return;
        CmdBuild(building);
        ControlProgress();
    }

    private void ControlProgress()
    {
        buildJob.Completed = progress >= maxProgress;
        if (buildJob.Completed)
            owner.CmdCreateBuilding(netId, buildingType);
    }

    public override string GetObjectDescription()
    {
        return $"progress {(int)progress}/{maxProgress}";
    }

    public override Job GetOwnJob(Commandable worker)
    {
        if (buildJob == null)
            buildJob = new JobBuild(this);
        return buildJob;
    }

    protected override void ShowAllButtons()
    {
        base.ShowAllButtons();
        UIManager.Instance.ShowDestroyButton();
    }

    protected override void HideAllButtons()
    {
        base.HideAllButtons();
        UIManager.Instance.HideDestroyButton();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerState.Instance?.temporaryBuildings.Remove(this);
        GameState.Instance?.RemoveFromSquare(SquareID, this);
        GameState.Instance?.TemporaryBuildings.Remove(this);
    }
}