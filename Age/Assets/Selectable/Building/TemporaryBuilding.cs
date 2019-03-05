using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TemporaryBuilding : Selectable
{
    static readonly int maxProgress = 100;
    [SyncVar(hook = "OnPlacedChange")]
    public bool placed = false;

    private Job buildJob = null;
    [SyncVar(hook = "OnProgressChange")]
    private float progress = 0;

    private void OnPlacedChange(bool placed)
    {
        gameObject.SetActive(true);
    }

    private void OnProgressChange(float newProgress)
    {
        gameState.OnStateChange(this);
        progress = newProgress;
    }

    [ClientRpc]
    public void RpcOnCreate(NetworkInstanceId workerID)
    {
        if (hasAuthority)
        {
            gameState.SetWorkerAndBuilding(this, ClientScene.objects[workerID].GetComponent<Commandable>());
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    [Command]
    private void CmdBuild(int strength)
    {
        progress += strength;
    }
    public void Build(Unit worker)
    {
        CmdBuild(worker.Strength);
        ControlProgress();
    }

    private void ControlProgress()
    {
        buildJob.Completed = progress >= maxProgress;
        if (buildJob.Completed)
            gameState.CmdCreateMainBuilding(netId);
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
        gameState.AddSelectable(this);
    }

    public override void DrawBottomBar(Text nameText, Text selectedObjectText)
    {
        nameText.text = "Temporary Building";
        selectedObjectText.text = string.Format("progress {0}/{1}", progress, maxProgress);
    }

    protected override Job CreateOwnJob(Commandable worker)
    {
        if (buildJob == null)
            buildJob = new JobBuild(this);
        return buildJob;
    }

    protected override Job CreateEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }
    public override void DrawHealthBar()
    {
        DrawProgressBar(progress / maxProgress);
    }
}