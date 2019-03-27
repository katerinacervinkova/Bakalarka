using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : Building {

    protected override Job GetOwnJob(Commandable worker)
    {
        return null;
    }

    protected override void InitTransactions()
    {
        Transactions.Add(new Transaction("Unit", "Create a unit", () => owner.CreateUnit(this), food: 20, wood: 0, gold: 0));
    }
}
