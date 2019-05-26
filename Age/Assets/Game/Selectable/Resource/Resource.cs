using Pathfinding;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Resource : Selectable {

    protected Job miningJob = null;

    [SyncVar(hook = "OnCapacityChange")]
    public float capacity = 0;

    public override float HealthValue => capacity / MaxCapacity;

    protected abstract float MaxCapacity { get; }
    public abstract void Gather(float gathering, Player player);

    protected void Start()
    {
        capacity = MaxCapacity;
    }

    public override void OnStartClient()
    {
        Init();
        initialized = true;
        minimapColor = minimapIcon.color;
    }

    private void OnCapacityChange(float newCapacity)
    {
        capacity = newCapacity;
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance?.OnStateChange(this);
            if (initialized && PlayerState.Instance?.SelectedObject != this)
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameState.Instance?.Resources.Remove(this);
        GameState.Instance?.UpdateGraph(GetComponent<Collider>().bounds);
    }
}
