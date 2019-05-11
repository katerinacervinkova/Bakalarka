using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public abstract class Selectable : NetworkBehaviour {

    public abstract string Name { get; }
    public Texture2D Image;

    [SyncVar]
    public NetworkInstanceId playerID;
    public Player owner;

    [SyncVar(hook = "OnHealthChange")]
    public float Health;
    [SyncVar]
    public float MaxHealth = 100;
    [SyncVar]
    protected float lineOfSight = 10;

    protected bool Selected { get; set; } = false;

    protected GameObject selector;
    protected SpriteRenderer minimapIcon;
    protected Color minimapColor;

    [SerializeField]
    public Vector3 size;

    [SerializeField]
    public float healthBarOffset;
    protected HealthBar healthBar;

    protected bool initialized = false;

    public Vector3 FrontPosition { get { var pos = transform.position; pos.x -= size.x / 2; pos.z -= size.z / 2; return pos; } }
    public virtual float HealthValue => Health / MaxHealth;
    public List<Purchase> Purchases { get; private set; } = new List<Purchase>();

    public abstract Job GetOwnJob(Commandable worker = null);
    public virtual Job GetEnemyJob(Commandable worker = null) => new AttackJob(this);
    protected abstract void InitPurchases();

    public override void OnStartClient()
    {
        owner = ClientScene.objects[playerID].GetComponent<Player>();
        Init();
        initialized = true;
    }

    public virtual void Init()
    {
        minimapIcon = transform.Find("MinimapIcon").GetComponent<SpriteRenderer>();
        selector = transform.Find("SelectionProjector").gameObject;
        selector.SetActive(false);
        Health = MaxHealth;
        healthBar = UIManager.Instance.CreateHealthBar(this, healthBarOffset);
    }

    public override void OnStartAuthority()
    {
        InitPurchases();
    }

    public virtual void SetSelection(bool selected, Player player)
    {
        Selected = selected;
        selector.SetActive(selected);
        minimapIcon.color = selected ? Color.white : minimapColor;
        if (healthBar != null)
        {
            if (selected)
                healthBar.Show();
            else
                healthBar.Hide();
        }
        if (hasAuthority)
        {
            if (selected)
                UIManager.Instance?.ShowButtons(Purchases);
            else
                UIManager.Instance?.HideButtons();
        }
    }

    public virtual string GetObjectDescription()
    {
        return $"Health: {(int)Health}/{(int)MaxHealth}";
    }

    public virtual void RightMouseClickGround(Vector3 hitPoint) { }
    public virtual void RightMouseClickObject(Selectable hitObject) { }
    public virtual Job CreateJob(Commandable worker)
    {
        if (owner == worker.owner)
            return GetOwnJob(worker);
        return GetEnemyJob(worker);
    }

    public bool IsWithinSight(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < lineOfSight;
    }
    
    protected virtual void OnHealthChange(float value)
    {
        Health = value;
        PlayerState.Instance.OnStateChange(this);
        if (initialized && PlayerState.Instance.SelectedObject != this)
            healthBar.HideAfter();
    }
    protected virtual void OnDestroy()
    {
        if (healthBar != null)
            Destroy(healthBar.gameObject);
        if (GetEnemyJob() != null)
            GetEnemyJob().Completed = true;
        if (GetOwnJob() != null)
            GetOwnJob().Completed = true;
        if (Selected)
            PlayerState.Instance?.Deselect();
    }
}