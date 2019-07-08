using UnityEngine;
using UnityEngine.Networking;

public abstract class Resource : Selectable {

    protected Job miningJob = null;

    private bool started = false;

    [SyncVar(hook = "OnCapacityChange")]
    public float capacity = 0;

    public override float HealthValue => capacity / MaxCapacity;

    protected abstract float MaxCapacity { get; }
    public abstract bool Gather(float gathering, Player player);

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
        started = true;
        healthBar = UIManager.Instance.CreateHealthBar(this, healthBarOffset);
        minimapColor = minimapIcon.color;
    }

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

    public override Job GetOwnJob(Commandable worker)
    {
        return GetEnemyJob(worker);
    }

    public override string GetObjectDescription()
    {
        return $"Capacity: {(int)capacity}/{(int)MaxCapacity}";
    }

    private void OnDisable()
    {
        if (started)
            OnDestroy();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameState.Instance?.Resources.Remove(this);
        GameState.Instance?.RemoveFromSquare(SquareID, this);
    }
}
