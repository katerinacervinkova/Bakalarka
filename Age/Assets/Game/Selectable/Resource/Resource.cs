using Pathfinding;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Resource : Selectable {

    protected Job miningJob = null;

    [SerializeField]
    public int size;

    [SyncVar(hook = "OnCapacityChange")]
    public float capacity = 0;
    protected abstract float MaxCapacity { get; }
    public abstract void Gather(float gathering, Player player);

    protected void Start()
    {
        capacity = MaxCapacity;
    }

    public override void OnStartClient()
    {
        Init();
        minimapColor = minimapIcon.color;
    }

    private void OnCapacityChange(float newCapacity)
    {
        capacity = newCapacity;
        PlayerState.Instance.OnStateChange(this);
        DrawHealthBar();
    }

    public override void DrawHealthBar()
    {
        DrawProgressBar((float)capacity / MaxCapacity);
    }

    public override Job GetOwnJob(Commandable worker)
    {
        return GetEnemyJob(worker);
    }

    public override string GetObjectDescription()
    {
        return $"Capacity: {(int)capacity}/{(int)MaxCapacity}";
    }

    protected override void InitPurchases() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameState.Instance?.Resources.Remove(this);
        GameState.Instance?.UpdateGraph(GetComponent<Collider>().bounds);
    }
}
