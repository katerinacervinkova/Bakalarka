using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public abstract class Selectable : NetworkBehaviour {

    [SyncVar]
    public string Name;
    public Texture2D Image;

    [SyncVar]
    public NetworkInstanceId playerID;
    public Player owner;

    [SyncVar]
    public float Health;
    [SyncVar]
    public float MaxHealth = 100;
    [SyncVar]
    protected float lineOfSight = 10;

    protected bool Selected { get; set; } = false;

    protected GameObject healthBarCanvas;
    protected GameObject selector;
    protected SpriteRenderer minimapIcon;
    protected Color minimapColor;

    protected Image healthBar;
    protected Quaternion healthBarRotation;

    public List<Purchase> Purchases { get; private set; } = new List<Purchase>();

    public abstract Job GetOwnJob(Commandable worker = null);
    public abstract Job GetEnemyJob(Commandable worker = null);
    public abstract void DrawHealthBar();
    protected abstract void InitPurchases();
    public abstract string GetObjectDescription();

    public override void OnStartClient()
    {
        owner = ClientScene.objects[playerID].GetComponent<Player>();
        Init();
    }

    public virtual void Init()
    {
        minimapIcon = transform.Find("MinimapIcon").GetComponent<SpriteRenderer>();
        selector = transform.Find("SelectionProjector").gameObject;
        selector.SetActive(false);
        healthBarCanvas = transform.Find("Canvas").gameObject;
        healthBar = transform.Find("Canvas/HealthBarBG/HealthBar").GetComponent<Image>();
        healthBarRotation = healthBarCanvas.transform.rotation;
        Health = MaxHealth;
    }

    public override void OnStartAuthority()
    {
        InitPurchases();
    }

    protected virtual void Update() { }

    public virtual void SetSelection(bool selected, Player player)
    {
        Selected = selected;
        selector.SetActive(selected);
        minimapIcon.color = selected ? Color.white : minimapColor;
        healthBarCanvas.SetActive(selected);
        if (selected)
            DrawHealthBar();
        if (hasAuthority)
        {
            if (selected)
                UIManager.Instance?.ShowButtons(Purchases);
            else
                UIManager.Instance?.HideButtons();
        }
    }

    public virtual void RightMouseClickGround(Vector3 hitPoint) { }
    public virtual void RightMouseClickObject(Selectable hitObject) { }
    public virtual Job CreateJob(Commandable worker)
    {
        if (owner == worker.owner)
            return GetOwnJob(worker);
        return GetEnemyJob(worker);
    }

    protected virtual void DrawProgressBar(float value)
    {
        healthBarCanvas.transform.rotation = healthBarRotation;
        healthBar.fillAmount = value;
    }

    public bool IsWithinSight(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < lineOfSight;
    }
    
    protected virtual void OnDestroy()
    {
        if (GetEnemyJob() != null)
            GetEnemyJob().Completed = true;
        if (GetOwnJob() != null)
            GetOwnJob().Completed = true;
        if (Selected)
            PlayerState.Instance?.Deselect();
    }
}