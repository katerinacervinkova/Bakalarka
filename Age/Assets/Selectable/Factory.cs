using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Factory : MonoBehaviour
{
    protected System.Random rnd;
    protected int sumOfProperties = 35;

    public Regiment regimentPrefab;
    public Unit unitPrefab;
    public Scheduler schedulerPrefab;
    public GridGraph gridGraph;

    public Image panel;
    protected Text selectedAttributesText;
    protected Text nameText;

    protected virtual void Start()
    {
        regimentPrefab.gameObject.SetActive(false);
        unitPrefab.gameObject.SetActive(false);
        schedulerPrefab.gameObject.SetActive(false);
        rnd = new System.Random();
        selectedAttributesText = panel.transform.Find("selectableAttributesText").GetComponent<Text>();
        nameText = panel.transform.Find("nameText").GetComponent<Text>();
    }

    public Regiment CreateRegiment(Player owner, List<Unit> units)
    {
        Regiment regiment = Instantiate(regimentPrefab);
        regiment.owner = owner;
        regiment.SetUnits(units);
        regiment.gridGraph = gridGraph;
        regiment.Name = string.Format("Units({0})", units.Count);
        regiment.gameObject.SetActive(true);
        return regiment;
    }

    public Unit CreateUnit(Player owner, Vector3 position, Vector3 destination)
    {

        Unit unit = Instantiate(unitPrefab, gridGraph.ClosestDestination(position), Quaternion.identity);

        unit.Name = "Unit";
        unit.owner = owner;
        unit.selectedObjectText = selectedAttributesText;
        unit.nameText = nameText;

        owner.units.Add(unit);
        SetRandomParameters(unit);
        unit.gameObject.SetActive(true);
        if (destination != position)
            unit.SetDestination(destination);
        return unit;
    }

    public Scheduler CreateScheduler(List<Scheduler> schedulers, Action action, Image image)
    {
        Vector3 position = new Vector3(-300, -30, 0);
        position.x += 50 * schedulers.Count;
        Scheduler scheduler = Instantiate(schedulerPrefab, position, Quaternion.identity);
        scheduler.transform.SetParent(panel.transform, false);
        scheduler.schedulers = schedulers;
        scheduler.ActionToPerform = action;
        scheduler.image = image;
        scheduler.Speed = 1f / 1;

        scheduler.gameObject.SetActive(true);
        return scheduler;
    }

    private void SetRandomParameters(Unit unit)
    {
        int health = rnd.Next(100);
        int strength = rnd.Next(100);
        int intelligence = rnd.Next(100);
        int agility = rnd.Next(100);
        int healing = rnd.Next(100);
        int crafting = rnd.Next(100);
        int accuracy = rnd.Next(100);

        int ratio = (health + strength + intelligence + agility + healing + crafting + accuracy);

        unit.MaxHealth = 100 + health * sumOfProperties / ratio + 1;
        unit.Health = unit.MaxHealth;
        unit.Strength = strength * sumOfProperties / ratio + 1;
        unit.Intelligence = intelligence * sumOfProperties / ratio + 1;
        unit.Agility = agility * sumOfProperties / ratio + 1;
        unit.Healing = healing * sumOfProperties / ratio + 1;
        unit.Crafting = crafting * sumOfProperties / ratio + 1;
        unit.Accuracy = accuracy * sumOfProperties / ratio + 1;
    }
}
