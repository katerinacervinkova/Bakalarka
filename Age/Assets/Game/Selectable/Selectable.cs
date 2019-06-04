using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public abstract class Selectable : NetworkBehaviour {

    public abstract string Name { get; }

    [SyncVar]
    public NetworkInstanceId playerID;
    public Player owner;

    [SyncVar(hook = "OnHealthChange")]
    public float Health;
    [SyncVar]
    public float MaxHealth = 100;
    [SyncVar]
    protected float lineOfSight = 10;

    protected GameObject visibleObject;
    protected GameObject selector;
    protected SpriteRenderer minimapIcon;
    protected Color minimapColor;

    [SerializeField]
    public Vector3 size;

    [SerializeField]
    public float healthBarOffset;
    protected HealthBar healthBar;
    protected bool initialized = false;

    public Vector2 SquareID { get; set; } = Vector2.positiveInfinity;
    public Vector3 FrontPosition { get { var pos = transform.position; pos.x -= size.x / 2; pos.z -= size.z / 2; return pos; } }
    public virtual float HealthValue => Health / MaxHealth;
    public List<Purchase> Purchases { get; private set; } = new List<Purchase>();

    public abstract Job GetOwnJob(Commandable worker = null);
    public virtual Job GetEnemyJob(Commandable worker = null) => new AttackJob(this);

    public override void OnStartClient()
    {
        owner = ClientScene.objects[playerID].GetComponent<Player>();
        Init();
        initialized = true;
    }

    public virtual void Init()
    {
        visibleObject = transform.Find("Visible").gameObject;
        minimapIcon = visibleObject.transform.Find("MinimapIcon").GetComponent<SpriteRenderer>();
        selector = visibleObject.transform.Find("SelectionProjector").gameObject;
        selector.SetActive(false);
        Health = MaxHealth;
    }

    public override void OnStartAuthority()
    {
        InitPurchases();
    }

    protected virtual void InitPurchases() { }

    public virtual void SetSelection(bool selected)
    {
        SetVisualSelection(selected);
        if (hasAuthority && UIManager.Instance != null)
        {
            if (selected)
                ShowAllButtons();
            else
                HideAllButtons();
        }
    }

    public virtual void SetVisualSelection(bool selected)
    {
        selector.SetActive(selected);
        minimapIcon.color = selected ? Color.white : minimapColor;
        if (healthBar != null)
        {
            if (selected)
                healthBar.Show();
            else
                healthBar.Hide();
        }
    }

    public void SetVisibility(bool visible)
    {
        if (visible)
            Uncover();
        else
            Cover();
    }

    protected virtual void Cover()
    {
        if (healthBar != null)
            healthBar.transform.localScale = new Vector3();
    }

    protected virtual void Uncover()
    {
        visibleObject.SetActive(true);
        if (healthBar != null)
            healthBar.transform.localScale = new Vector3(2, 2, 2);
    }

    protected virtual void ShowAllButtons()
    {
        UIManager.Instance.ShowPurchaseButtons(Purchases);
    }

    protected virtual void HideAllButtons()
    {
        UIManager.Instance.HidePurchaseButtons();
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
        if (PlayerState.Instance != null)
        {
            if (initialized && PlayerState.Instance.SelectedObject != this)
                healthBar.HideAfter();
            PlayerState.Instance.OnStateChange(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (healthBar != null)
            Destroy(healthBar.gameObject);
        if (GetEnemyJob() != null)
            GetEnemyJob().Completed = true;
        if (GetOwnJob() != null)
            GetOwnJob().Completed = true;
        if (PlayerState.Instance != null && PlayerState.Instance.SelectedObject == this)
            PlayerState.Instance.Deselect();
    }
}