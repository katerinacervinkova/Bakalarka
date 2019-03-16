using System;

public class Transaction
{
    public string name;
    public int gold;
    public Action action;

    public Transaction(string name, int gold, Action action)
    {
        this.name = name;
        this.gold = gold;
        this.action = action;
    }

    public void Do(PlayerState pState)
    {
        if (pState.PayGold(gold))
            action.Invoke();
    }
}
