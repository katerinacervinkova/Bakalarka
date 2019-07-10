using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Base class for all objects that can be selected.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class Selectable : NetworkBehaviour {

    public abstract string Name { get; }

    protected bool initialized = false;

    public int playerId;
    [SyncVar]
    public NetworkInstanceId playerNetId;
    public Player owner;

    [SyncVar(hook = "OnHealthChange")]
    public float Health;
    [SyncVar]
    public float MaxHealth = 100;
    public virtual float HealthValue => Health / MaxHealth;

    [SerializeField]
    public float healthBarOffset;
    protected HealthBar healthBar;

    protected GameObject visibleObject;
    protected GameObject selector;
    protected SpriteRenderer minimapIcon;
    protected Color minimapColor;

    [SerializeField]
    public Vector3 size;
    public Vector2 SquareID { get; set; } = Vector2.positiveInfinity;
    public Vector3 FrontPosition { get { var pos = transform.position; pos.x -= size.x / 2; pos.z -= size.z / 2; return pos; } }

    public List<Purchase> Purchases { get; private set; } = new List<Purchase>();

    /// <summary>
    /// Returns job for a unit that belongs to the same player.
    /// </summary>
    public abstract Job GetOwnJob(Unit worker = null);
    /// <summary>
    /// Returns job for an enemy unit.
    /// </summary>
    public virtual Job GetEnemyJob(Unit worker = null) => new JobAttack(this);

    public override void OnStartClient()
    {
        owner = ClientScene.objects[playerNetId].GetComponent<Player>();
        playerId = owner.playerControllerId;
        Init();
        initialized = true;
    }

    /// <summary>
    /// Initializes the object.
    /// </summary>
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

    /// <summary>
    /// Adds purchases to the list.
    /// </summary>
    protected virtual void InitPurchases() { }

    /// <summary>
    /// Adds purchase of given type to the list.
    /// </summary>
    public void AddPurchase(PurchasesEnum purchasesEnum)
    {
        AddPurchase(PlayerState.Get(playerId).playerPurchases.Get(purchasesEnum));
    }

    /// <summary>
    /// Adds given purchase to the list.
    /// </summary>
    public void AddPurchase(Purchase purchase)
    {
        Purchases.Add(purchase);
        PlayerState.Get().OnStateChange(this);
    }

    /// <summary>
    /// Removes given purchase from the list.
    /// </summary>
    public void RemovePurchase(Purchase purchase)
    {
        Purchases.Remove(purchase);
        PlayerState.Get().OnStateChange(this);
    }

    /// <summary>
    /// Selects or deselects object. Shows visually that it was (de)selected and if that object belongs to player, shows the corresponding buttons.
    /// </summary>
    /// <param name="selected">true if the object was just selected, false if it was deselected</param>
    public virtual void SetSelection(bool selected)
    {
        SetVisualSelection(selected);
        if ((owner == null || owner.IsHuman) && playerId == 0 && hasAuthority && UIManager.Instance != null)
        {
            if (selected)
                ShowAllButtons();
            else
                HideAllButtons();
        }
    }

    /// <summary>
    /// Selects or deselect object visually by turning on/of the projector above it, minimap icon and health bar.
    /// </summary>
    /// <param name="selected">true if the object was just selected, false if it was deselected</param>
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

    /// <summary>
    /// Makes the object visible or invisible.
    /// </summary>
    /// <param name="visible">true if object is to be visible, false if invisible</param>
    public void SetVisibility(bool visible)
    {
        if (visible)
            Uncover();
        else
            Cover();
    }

    /// <summary>
    /// Hides the health bar.
    /// </summary>
    protected virtual void Cover()
    {
        if (healthBar != null)
            healthBar.transform.localScale = new Vector3();
    }

    /// <summary>
    /// Shows the visible object and the health bar.
    /// </summary>
    protected virtual void Uncover()
    {
        if (visibleObject != null)
            visibleObject.SetActive(true);
        if (healthBar != null)
            healthBar.transform.localScale = new Vector3(2, 2, 2);
    }

    /// <summary>
    /// Check if some of the purchases are now (un)available.
    /// </summary>
    protected virtual void Update()
    {
        foreach (Purchase purchase in Purchases)
        {
            if (purchase.ActiveChanged(this))
                PlayerState.Get().OnStateChange(purchase);
        }
    }

    /// <summary>
    /// Show all purchase buttons with available actions.
    /// </summary>
    protected virtual void ShowAllButtons()
    {
        UIManager.Instance.ShowPurchaseButtons(Purchases, this);
    }

    protected virtual void HideAllButtons()
    {
        UIManager.Instance.HidePurchaseButtons();
    }

    /// <summary>
    /// Returns string describing the health of the object.
    /// </summary>
    public virtual string GetObjectDescription()
    {
        return $"Health: {(int)Health}/{(int)MaxHealth}";
    }

    /// <summary>
    /// Calls when the object is being attacked. 
    /// </summary>
    /// <param name="selectable">selectable who performed the attack</param>
    public virtual void DealAttack(Selectable selectable) { }
    /// <summary>
    /// Called when the player has just right-clicked on the ground and this object was selected.
    /// </summary>
    /// <param name="hitPoint">world position of the click</param>
    public virtual void SetGoal(Vector3 hitPoint) { }
    /// <summary>
    /// Called when the player has just right-clicked on the given selectable and this object was selected.
    /// </summary>
    /// <param name="hitObject">object that the user clicked at</param>
    public virtual void SetGoal(Selectable hitObject) { }
    
    /// <summary>
    /// Called when the value of health changes.
    /// </summary>
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