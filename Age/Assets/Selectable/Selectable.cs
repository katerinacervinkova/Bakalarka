using UnityEngine;
using Age;

public abstract class Selectable : MonoBehaviour {

    public string Name;
    public Texture2D Image;

    public Player owner;
    protected GameObject selector;
    protected bool selected = false;

    protected virtual void Start()
    {
        selector = transform.Find("SelectionProjector").gameObject;
    }

    protected virtual void Update()
    {

    }

    public virtual void SetSelection(bool selected, Player player)
    {
        if (this.selected == selected)
            return;
        this.selected = selected;
        if (selected)
            selector.GetComponent<Projector>().material.color = player.color;
        selector.SetActive(selected);
    }

    public virtual void PerformAction(string actionToPerform)
    {
    }

    public virtual void RightMouseClick(GameObject hitObject, Vector3 hitPoint)
    {

    }
}