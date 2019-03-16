﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public abstract class Selectable : NetworkBehaviour {

    protected GameState gameState;
    protected PlayerState playerState;

    [SyncVar]
    public string Name;
    public Texture2D Image;

    [SyncVar]
    public NetworkInstanceId playerID;
    public Player owner;

    [SyncVar]
    public int Health;
    [SyncVar]
    public int MaxHealth = 100;

    protected bool Selected { get; set; } = false;

    protected GameObject healthBarCanvas;
    protected GameObject selector;
    protected Image healthBar;
    protected Quaternion healthBarRotation;

    public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

    public abstract void DrawBottomBar(Text nameText, Text selectedObjectText);
    protected abstract Job GetOwnJob(Commandable worker);
    protected abstract Job GetEnemyJob(Commandable worker);
    public abstract void DrawHealthBar();
    protected abstract void InitTransactions();

    public override void OnStartClient()
    {
        base.OnStartClient();
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        playerState = GameObject.Find("PlayerState").GetComponent<PlayerState>();
        owner = ClientScene.objects[playerID].GetComponent<Player>();
        selector = transform.Find("SelectionProjector").gameObject;
        selector.GetComponent<Projector>().material.color = owner.color;
        selector.SetActive(false);
        healthBarCanvas = transform.Find("Canvas").gameObject;
        healthBar = transform.Find("Canvas/HealthBarBG/HealthBar").GetComponent<Image>();
        healthBarRotation = healthBarCanvas.transform.rotation;
        Health = MaxHealth;
    }

    public override void OnStartAuthority()
    {
        InitTransactions();
    }

    protected virtual void Update() { }

    public virtual void SetSelection(bool selected, Player player, BottomBar bottomBar = null)
    {
        Selected = selected;
        selector.SetActive(selected);
        healthBarCanvas.SetActive(selected);
        if (selected)
            DrawHealthBar();
        if (hasAuthority && bottomBar)
            bottomBar.SetActive(Transactions, selected);
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

    public virtual void RemoveBottomBar(Text nameText, Text selectedObjectText) { }

    protected virtual void OnDestroy()
    {
        if (GetEnemyJob(null) != null)
         GetEnemyJob(null).Completed = true;
        if (GetOwnJob(null) != null)
            GetOwnJob(null).Completed = true;
        if (Selected)
            playerState.Deselect();
    }
}