using System;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Resource : Selectable {

    protected Job miningJob = null;

    // own job and enemy job are the same for everyone
    public override Job GetOwnJob(Unit worker) => GetEnemyJob(worker);

    public override string GetObjectDescription() => $"Capacity: {(int)capacity}/{(int)MaxCapacity}";


    [SyncVar(hook = "OnCapacityChange")]
    public float capacity = 0;
    protected abstract float MaxCapacity { get; }

    /// <summary>
    /// value to be shown on the healthbar.
    /// </summary>
    public override float HealthValue => capacity / MaxCapacity;

    public virtual bool Gather(float gathering, Player player)
    {
        bool completed = capacity - gathering <= 0;
        player.Gather(Math.Min(gathering, capacity), this);
        return completed;
    }

    public override void OnStartClient()
    {
        Init();
        initialized = true;
    }

    public override void Init()
    {
        base.Init();
        capacity = MaxCapacity;
        visibleObject.SetActive(false);
        SetVisibility(false);
        healthBar = UIManager.Instance.CreateHealthBar(this, healthBarOffset);
        minimapColor = minimapIcon.color;
    }

    /// <summary>
    /// Shows health bar for a while when the capacity changes
    /// </summary>
    private void OnCapacityChange(float newCapacity)
    {
        capacity = newCapacity;
        if (PlayerState.Get() != null)
        {
            PlayerState.Get()?.OnStateChange(this);
            if (initialized && PlayerState.Get()?.SelectedObject != this)
                healthBar.HideAfter();
        }
    }

    private void OnDisable()
    {
        if (initialized)
            OnDestroy();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameState.Instance?.Resources.Remove(this);
        GameState.Instance?.RemoveFromSquare(SquareID, this);
    }
}
