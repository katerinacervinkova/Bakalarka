using System.Collections.Generic;
using UnityEngine;

public class MapSquare : MonoBehaviour {

    public bool activated = false;
    public bool wasActive = false;

    private bool uncovered = false;

    [SerializeField]
    private GameObject transparent;
    [SerializeField]
    private GameObject nontransparent;
    [SerializeField]
    private GameObject transparentMinimap;
    [SerializeField]
    private GameObject nontransparentMinimap;

    public List<MapSquare> AdjoiningSquares;

    public List<Selectable> Enemies { get; private set; } = new List<Selectable>();
    public List<Selectable> Friends { get; private set; } = new List<Selectable>();

    public bool ContainsFriend => Friends.Count > 0;


    public void Add(Selectable selectable)
    {
        if (selectable.hasAuthority)
            Friends.Add(selectable);
        else
            Enemies.Add(selectable);
    }


    public void Activate()
    {
        AdjoiningSquares.ForEach(s => s.activated = true);
    }

    public void UpdateVisibility()
    {
        if (activated && !wasActive)
            Uncover();
        else if (!activated && wasActive)
            Cover();
        activated = false;
    }

    private void Cover()
    {
        ActivateTransparent(true);
    }

    private void Uncover()
    {
        if (!uncovered)
        {
            Destroy(nontransparent);
            Destroy(nontransparentMinimap);
            uncovered = true;
        }
        ActivateTransparent(false);
    }

    private void ActivateTransparent(bool active)
    {
        transparent.SetActive(active);
        transparentMinimap.SetActive(active);
        wasActive = !active;
    }

    public void Remove(Selectable selectable)
    {
        if (selectable.hasAuthority)
            Friends.Remove(selectable);
        else
            Enemies.Remove(selectable);
    }
}