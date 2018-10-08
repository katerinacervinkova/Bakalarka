using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class Selectable : MonoBehaviour {

    public string Name;
    public Texture2D Image;
    public List<Button> buttons;
    public Text selectedObjectText;
    public Text nameText;

    public Player owner;
    protected GameObject selector;
    protected bool selected = false;
    protected virtual void Awake()
    {
        selector = transform.Find("SelectionProjector").gameObject;
        selector.GetComponent<Projector>().material.color = owner.color;
    }

    protected virtual void Update()
    {

    }

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
        foreach(var button in buttons)
            button.gameObject.SetActive(selected);
        if (selected)
            DrawBottomBar();
        nameText.gameObject.SetActive(selected);
        selectedObjectText.gameObject.SetActive(selected);
    }
    public virtual void RightMouseClickGround(GameObject hitObject, Vector3 hitPoint)
    {

    }
    public virtual void RightMouseClickObject(Vector3 hitpoint)
    {
        
    }
    public abstract void DrawBottomBar();
}