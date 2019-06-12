using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour {

    public AIPlayer aiPlayer;

    bool hasBuilding = false;

    void Update () {

        if (!hasBuilding)
            hasBuilding = aiPlayer.BuildBuilding(BuildingEnum.MainBuilding, new Vector3());
        else
        {
            aiPlayer.BuildUnit();
            aiPlayer.SenseIdleUnits().ForEach(u => aiPlayer.Gather<FoodResource>(u));
        }
    }
}
