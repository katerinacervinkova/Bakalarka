using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class Selectable : MonoBehaviour {

    public string Name;
    public Texture2D Image;
    public Text selectedObjectText;
    public Text nameText;
    public GridGraph gridGraph;
    public BottomBar bottomBar;

    public Player owner;
    protected GameObject selector;
    protected bool selected = false;
    protected bool selectedByOwner = false;

    public abstract void DrawBottomBar();
    protected abstract Job CreateOwnJob(Commandable worker);
    protected abstract Job CreateEnemyJob(Commandable worker);

    protected virtual void Awake()
    {
        selector = transform.Find("SelectionProjector").gameObject;
        selector.GetComponent<Projector>().material.color = owner.color;
        gridGraph = GameObject.Find("Map").GetComponent<GridGraph>();

    }
    
    protected virtual void Start()
    {
        gridGraph.AddSelectable(this);
    }

    protected virtual void Update() { }

    public virtual void SetSelection(bool selected, Player player)
    {
        if (this.selected == selected)
            return;
        this.selected = selected;
        selector.SetActive(selected);
        BottomBarUI(selected, player);
    }

    protected virtual void BottomBarUI(bool selected, Player player)
    {
        if (selected)
            DrawBottomBar();
        nameText.gameObject.SetActive(selected);
        selectedObjectText.gameObject.SetActive(selected);
    }
    public virtual void RightMouseClickGround(Vector3 hitPoint)
    {

    }
    public virtual void RightMouseClickObject(Selectable hitObject)
    {
        
    }
    public virtual Job CreateJob(Commandable worker)
    {
        if (owner == worker.owner)
            return CreateOwnJob(worker);
        return CreateEnemyJob(worker);
    }

    protected virtual void DrawProgressBar(float value)
    {
        Bounds b = GetComponent<Collider>().bounds;
        Vector3 position = Camera.main.WorldToScreenPoint(new Vector3(b.center.x, b.center.y + b.extents.y, b.center.z));
        EditorGUI.ProgressBar(new Rect(position.x - 30, owner.gameWindow.TopBorder - (position.y), 60, 8), value, "");
    }
}