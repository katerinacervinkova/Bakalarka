using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TemporaryBuilding : Selectable
{
    static readonly int maxProgress = 100;
    [SyncVar]
    public bool placed = false;

    private Job buildJob = null;
    [SyncVar(hook = "OnProgressChange")]
    private float progress = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        transform.Find("Floor1").GetComponent<MeshRenderer>().material.color = owner.color;
        minimapColor = owner.color;
        minimapIcon.color = minimapColor;

    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        playerState.SetWorkerAndBuilding(this);
        playerState.temporaryBuildings.Add(this);
    }

    private void OnProgressChange(float newProgress)
    {
        progress = newProgress;
        playerState.OnStateChange(this);
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
    private void CmdBuild(int strength)
    {
        progress += Math.Min(maxProgress - progress, strength);
    }
    public void Build(Unit worker)
    {
        if (!hasAuthority)
            return;
        CmdBuild(worker.Crafting);
        ControlProgress();
    }

    private void ControlProgress()
    {
        buildJob.Completed = progress >= maxProgress;
        if (buildJob.Completed)
            owner.CmdCreateBuilding(netId);
    }

    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        nameText.text = "Temporary Building";
        selectedObjectText.text = string.Format("progress {0}/{1}", progress, maxProgress);
    }

    protected override Job GetOwnJob(Commandable worker)
    {
        if (buildJob == null)
            buildJob = new JobBuild(this);
        return buildJob;
    }

    protected override Job GetEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar(progress / maxProgress);
    }

    protected override void InitTransactions()
    {
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        playerState.temporaryBuildings.Remove(this);
    }
}