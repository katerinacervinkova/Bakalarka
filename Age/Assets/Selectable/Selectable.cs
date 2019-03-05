using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public abstract class Selectable : NetworkBehaviour {

    protected GameState gameState;

    [SyncVar]
    public string Name;
    public Texture2D Image;

    [SyncVar]
    public NetworkInstanceId playerID;
    public Player owner;

    [SyncVar]
    public int Health = 50;
    [SyncVar]
    public int MaxHealth = 100;

    protected bool Selected { get; set; } = false;

    protected GameObject healthBarCanvas;
    protected GameObject selector;
    protected Image healthBar;
    private Quaternion healthBarRotation;

    public abstract void DrawBottomBar(Text nameText, Text selectedObjectText);
    protected abstract Job CreateOwnJob(Commandable worker);
    protected abstract Job CreateEnemyJob(Commandable worker);
    public abstract void DrawHealthBar();

    public override void OnStartClient()
    {
        base.OnStartClient();
        owner = ClientScene.objects[playerID].GetComponent<Player>();
        gameState = owner.GetComponent<GameState>();
        selector = transform.Find("SelectionProjector").gameObject;
        selector.SetActive(false);
        healthBarCanvas = transform.Find("Canvas").gameObject;
        healthBar = transform.Find("Canvas/HealthBarBG/HealthBar").GetComponent<Image>();
        healthBarRotation = healthBarCanvas.transform.rotation;
        selector.GetComponent<Projector>().material.color = owner.color;
    }



    protected virtual void Update()
    {
    }

    public virtual void SetSelection(bool selected, Player player, BottomBar bottomBar)
    {
        Selected = selected;
        selector.SetActive(selected);
        healthBarCanvas.SetActive(selected);
        if (hasAuthority)
            bottomBar.SetActive(gameState, this, selected);
    }


    public virtual void RightMouseClickGround(Vector3 hitPoint) { }
    public virtual void RightMouseClickObject(Selectable hitObject) { }
    public virtual Job CreateJob(Commandable worker)
    {
        if (owner == worker.owner)
            return CreateOwnJob(worker);
        return CreateEnemyJob(worker);
    }

    protected virtual void DrawProgressBar(float value)
    {
        healthBarCanvas.transform.rotation = healthBarRotation;
        healthBar.fillAmount = value;
    }

    public virtual void RemoveBottomBar(Text nameText, Text selectedObjectText) { }

    protected virtual void OnDestroy()
    {
        if (Selected)
            gameState.Deselect();
    }
}