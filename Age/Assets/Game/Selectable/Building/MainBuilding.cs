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
        Transactions.Add(new Transaction("Create a unit", 20, () => owner.CreateUnit(this)));
    }
}
