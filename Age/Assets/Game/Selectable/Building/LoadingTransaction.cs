using System;

public class LoadingPurchase : Purchase {

    public Building building;

    private readonly float Speed;

	public LoadingPurchase(float Speed, Building building, string Name, string description, Action action, int food, int wood, int gold, int population = 0)
        : base(Name, description, action, food, wood, gold, population)
    {
        this.Speed = Speed;
        this.building = building;
    }

    public override void Do()
    {
        if (building.CanStartTransaction() && PlayerState.Instance.Pay(food, wood, gold, population))
                building.AddTransaction(new Transaction(this, Speed));
    }

    public void InvokeAction()
    {
        action.Invoke();
    }

    public void Reset()
    {
        PlayerState.Instance.Pay(-food, -wood, -gold, -population);
    }
}
