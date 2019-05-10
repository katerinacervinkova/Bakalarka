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


    public override string Name => buildingType.ToString();
    public override float HealthValue => progress / maxProgress;

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
        ChangeColor();
    }

    private void ChangeColor()
    {
        switch (buildingType)
        {
            case BuildingEnum.MainBuilding:
                transform.Find("MainBuilding/Main Roof").GetComponent<MeshRenderer>().material.color = owner.color;
                transform.Find("MainBuilding/Roof 1").GetComponent<MeshRenderer>().material.color = owner.color;
                transform.Find("MainBuilding/Roof 2").GetComponent<MeshRenderer>().material.color = owner.color;
                break;
            case BuildingEnum.Library:
                transform.Find("Library/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
                break;
            case BuildingEnum.Barracks:
                transform.Find("Barracks/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
                transform.Find("Barracks/Dog").GetComponent<MeshRenderer>().material.color = owner.color;
                break;
            case BuildingEnum.Infirmary:
                transform.Find("Infirmary/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
                break;
            case BuildingEnum.House:
                transform.Find("House/Roof").GetComponent<MeshRenderer>().material.color = owner.color;
                break;
            default:
                break;
        }
    }

    private void OnProgressChange(float newProgress)
    {
        progress = newProgress;
        PlayerState.Instance.OnStateChange(this);
        if (initialized && PlayerState.Instance.SelectedObject != this)
            healthBar.HideAfter();
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