using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public abstract class Selectable : MonoBehaviour {

    public static readonly string healthEvent = "HealthChange";
    public static readonly string selectOwnEvent = "SelectOwn";
    public static readonly string deselectOwnEvent = "DeselectOwn";

    public string Name;
    public Texture2D Image;
    public Text selectedObjectText;
    public Text nameText;
    public GridGraph gridGraph;
    public BottomBar bottomBar;

    public Player owner;
    private int health = 50;
    public int Health { get { return health; } set { health = value; EventManager.TriggerEvent(this, healthEvent); } }
    public int MaxHealth { get; set; } = 100;
    protected GameObject healthBarCanvas;
    protected GameObject selector;
    protected Image healthBar;
    protected bool selected = false;
    protected bool selectedByOwner = false;
    private Quaternion healthBarRotation;

    protected abstract void DrawSelectedObjectText();
    protected abstract void DrawNameText();
    protected abstract Job CreateOwnJob(Commandable worker);
    protected abstract Job CreateEnemyJob(Commandable worker);
    public abstract void DrawHealthBar();
    protected abstract void SetEvents();
    protected abstract void RemoveEvents();

    protected virtual void Awake()
    {
        selector = transform.Find("SelectionProjector").gameObject;
        selector.GetComponent<Projector>().material.color = owner.color;
        gridGraph = GameObject.Find("Map").GetComponent<GridGraph>();
        healthBarCanvas = transform.Find("Canvas").gameObject;
        healthBar = transform.Find("Canvas/HealthBarBG/HealthBar").GetComponent<Image>();
        healthBarRotation = healthBarCanvas.transform.rotation;
        SetEvents();
        SetHealthEvents();
        SetSelectionEvents();
    }

    protected virtual void SetHealthEvents()
    {
        EventManager.StartListening(this, healthEvent, DrawHealthBar);
        EventManager.StartListening(this, healthEvent, DrawSelectedObjectText);
    }

    protected virtual void SetSelectionEvents()
    {
        EventManager.StartListening(this, selectOwnEvent, SelectOwn);
        EventManager.StartListening(this, selectOwnEvent, DrawBottomBar);
        EventManager.StartListening(this, deselectOwnEvent, DeselectOwn);
    }

    protected void OnDestroy()
    {
        RemoveEvents();
        RemoveHealthEvents();
        RemoveSelectionEvents();
    }

    protected virtual void RemoveHealthEvents()
    {
        EventManager.StopAllListening(this, healthEvent);
    }
    protected virtual void RemoveSelectionEvents()
    {
        EventManager.StopAllListening(this, selectOwnEvent);
        EventManager.StopAllListening(this, deselectOwnEvent);
    }

    protected virtual void Start()
    {
        gridGraph.AddSelectable(this);
    }


    protected virtual void Update()
    {
    }

    public virtual void SelectOwn()
    {
        SetSelection(true, owner);
    }

    public virtual void DeselectOwn()
    {
        SetSelection(false, owner);
    }
    protected virtual void SetSelection(bool selected, Player player)
    {
        if (this.selected == selected)
            return;
        this.selected = selected;
        selector.SetActive(selected);
        healthBarCanvas.SetActive(selected);
        BottomBarUI(selected, player);
    }

    protected virtual void BottomBarUI(bool selected, Player player)
    {
        nameText.gameObject.SetActive(selected);
        selectedObjectText.gameObject.SetActive(selected);
    }

    protected virtual void DrawBottomBar()
    {
        DrawNameText();
        DrawSelectedObjectText();
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

}