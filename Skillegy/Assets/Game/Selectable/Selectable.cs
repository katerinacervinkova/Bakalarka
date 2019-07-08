using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public abstract class Selectable : NetworkBehaviour {

    public abstract string Name { get; }

    public int playerId;

    [SyncVar]
    public NetworkInstanceId playerNetId;
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
    public virtual Job GetEnemyJob(Commandable worker = null) => new JobAttack(this);

    public override void OnStartClient()
    {
        owner = ClientScene.objects[playerNetId].GetComponent<Player>();
        playerId = owner.playerControllerId;
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

    public void AddPurchase(PurchasesEnum purchasesEnum)
    {
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(purchasesEnum));
    }

    public void AddPurchase(Purchase purchase)
    {
        Purchases.Add(purchase);
        PlayerState.Get().OnStateChange(this);
    }

    public void RemovePurchase(Purchase purchase)
    {
        Purchases.Remove(purchase);
        PlayerState.Get().OnStateChange(this);
    }

    public virtual void SetSelection(bool selected)
    {
        SetVisualSelection(selected);
        if ((owner == null || owner.IsHuman) && hasAuthority && UIManager.Instance != null)
        {
            if (selected)
                ShowAllButtons();
            else
                HideAllButtons();
        }
    }

    public virtual void SetVisualSelection(bool selected)
    {
        if (selector != null)
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
        if (visibleObject != null)
            visibleObject.SetActive(true);
        if (healthBar != null)
            healthBar.transform.localScale = new Vector3(2, 2, 2);
    }

    protected virtual void Update()
    {
        foreach (Purchase purchase in Purchases)
        {
            if (purchase.ActiveChanged(this))
                PlayerState.Get().OnStateChange(purchase);
        }
    }

    protected virtual void ShowAllButtons()
    {
        UIManager.Instance.ShowPurchaseButtons(Purchases, this);
    }

    protected virtual void HideAllButtons()
    {
        UIManager.Instance.HidePurchaseButtons();
    }

    public virtual string GetObjectDescription()
    {
        return $"Health: {(int)Health}/{(int)MaxHealth}";
    }


    public virtual void DealAttack(Selectable selectable) { }
    public virtual void RightMouseClickGround(Vector3 hitPoint) { }
    public virtual void RightMouseClickObject(Selectable hitObject) { }

    public bool IsWithinSight(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < lineOfSight;
    }
    
    protected virtual void OnHealthChange(float value)
    {
        Health = value;
        if (PlayerState.Get() != null)
        {
            if (initialized && PlayerState.Get().SelectedObject != this)
                healthBar.HideAfter();
            PlayerState.Get().OnStateChange(this);
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
        if (playerId >= 0 && PlayerState.Get(playerId) != null && PlayerState.Get(playerId).SelectedObject == this)
            PlayerState.Get(playerId).Deselect();
    }
}