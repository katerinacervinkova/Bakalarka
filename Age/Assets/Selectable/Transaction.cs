using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public void Do(Player player)
    {
        if (!player.PayGold(gold))
            return;
        action.Invoke();
    }
}
