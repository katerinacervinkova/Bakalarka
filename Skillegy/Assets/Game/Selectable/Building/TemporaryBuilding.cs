using System;
using UnityEngine;
using UnityEngine.Networking;

public class TemporaryBuilding : Selectable
{
    public BuildingEnum buildingType;

    [SyncVar(hook = "OnProgressChange")]
    private float progress = 0;
    [SerializeField]
    private int maxProgress;
    [SyncVar]
    public bool placed = false;

    private Job buildJob = null;

    public override string Name => buildingType.ToString();
    // value to be shown on the health bar
    public override float HealthValue => progress / maxProgress;
    public override string GetObjectDescription() => $"progress {(int)progress}/{maxProgress}";

    private Collider coll;
    public Bounds Bounds => coll.bounds;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // temporary building should be visible for its owner
        gameObject.SetActive(true);
        if (playerId == 0)
            SetVisibility(true);

        PlayerState.Get(playerId).SetTempBuilding(this);
        PlayerState.Get(playerId).temporaryBuildings.Add(this);
    }

    public override void Init()
    {
        base.Init();
        minimapColor = owner.color;
        minimapIcon.color = minimapColor;
        visibleObject.transform.Find("Image").GetComponent<SpriteRenderer>().color = owner.color;

        GameState.Instance.TemporaryBuildings.Add(this);

        coll = GetComponent<Collider>();
        healthBar = UIManager.Instance.CreateHealthBar(this, healthBarOffset);

        //for everybody else than the owner the temporary building should be invisible
        gameObject.SetActive(false);
        visibleObject.SetActive(false);
        SetVisibility(false);
    }

    private void OnProgressChange(float newProgress)
    {
        progress = newProgress;
        PlayerState.Get()?.OnStateChange(this);
        if (initialized && PlayerState.Get()?.SelectedObject != this && healthBar != null)
            healthBar.HideAfter();
    }

    /// <summary>
    /// Show the building in progress.
    /// </summary>
    /// <param name="position">the desired location of the building</param>
    public void OnPlaced(Vector3 position)
    {
        transform.position = position;
        coll.enabled = true;
        visibleObject.transform.Find("Building").gameObject.SetActive(false);
        visibleObject.transform.Find("Fence").gameObject.SetActive(true);
        visibleObject.transform.Find("Image").gameObject.SetActive(true);
        gameObject.SetActive(true);
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
        // progress has to be synchronized, i.e. updated on the server
        CmdBuild(building);
        ControlProgress();
    }

    /// <summary>
    /// Creates building if the building process has reached its end.
    /// </summary>
    private void ControlProgress()
    {
        buildJob.Completed = progress >= maxProgress;
        if (buildJob.Completed)
            owner.CmdCreateBuilding(netId, buildingType);
    }

    /// <summary>
    /// Transfers health bar from the temporary building to the real building.
    /// </summary>
    /// <param name="building">building to transfer the health bar to</param>
    /// <returns>the health bar</returns>
    public HealthBar TransferHealthBar(Building building)
    {
        healthBar.selectable = building;
        return healthBar;
    }

    public override Job GetOwnJob(Unit worker)
    {
        if (buildJob == null)
            buildJob = new JobBuild(this, playerId);
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
        if (hasAuthority)
            PlayerState.Get(playerId)?.temporaryBuildings.Remove(this);
        GameState.Instance?.RemoveFromSquare(SquareID, this);
        GameState.Instance?.TemporaryBuildings.Remove(this);
    }
}