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

    [SerializeField]
    public Vector2Int posDelta;

    public override string Name => buildingType.ToString();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (buildingType == BuildingEnum.MainBuilding)
            transform.Find("Floor1").GetComponent<MeshRenderer>().material.color = owner.color;
        minimapColor = owner.color;
        minimapIcon.color = minimapColor;

    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        PlayerState.Instance.SetTempBuilding(this);
        PlayerState.Instance.temporaryBuildings.Add(this);
    }

    private void OnProgressChange(float newProgress)
    {
        progress = newProgress;
        PlayerState.Instance.OnStateChange(this);
        DrawHealthBar();
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

    public override Job GetEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar(progress / maxProgress);
    }

    protected override void InitPurchases()
    {
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerState.Instance?.temporaryBuildings.Remove(this);
        GameState.Instance?.TemporaryBuildings.Remove(this);
        GameState.Instance?.UpdateGraph(GetComponent<Collider>().bounds);
    }
}