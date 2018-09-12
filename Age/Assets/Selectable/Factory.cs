using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public Regiment regimentPrefab;
    // Use this for initialization
    public Regiment CreateRegiment(Player owner, List<Unit> units)
    {
        Regiment regiment = Instantiate(regimentPrefab);
        regiment.owner = owner;
        regiment.units = units;
        regiment.Name = string.Join(", ", units.Select(unit => unit.Name).ToArray());
        return regiment;
    }
}
