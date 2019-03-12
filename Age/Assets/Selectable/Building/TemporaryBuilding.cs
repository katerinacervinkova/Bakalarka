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

    }

    private void OnProgressChange(float newProgress)
    {
        progress = newProgress;
        gameState.OnStateChange(this);
        DrawHealthBar();
    }

    [ClientRpc]
    public void RpcOnCreate()
    {
        if (hasAuthority)
        {
            gameState.SetWorkerAndBuilding(this);
            gameObject.SetActive(true);
        }
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
        CmdBuild(worker.Strength);
        ControlProgress();
    }

    private void ControlProgress()
    {
        buildJob.Completed = progress >= maxProgress;
        if (buildJob.Completed)
            gameState.CreateMainBuilding(this);
    }

    [Command]
    public void CmdPlaceBuilding(Vector3 position)
    {
        transform.position = position;
        RpcSetPosition(position);
        placed = true;
    }

    [ClientRpc]
    private void RpcSetPosition(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        GetComponent<Collider>().enabled = true;
        gameState.AddSelectable(this);
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
}