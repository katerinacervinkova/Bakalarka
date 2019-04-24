using System;

public class LoadingPurchase : Purchase {

    public Building building;

    private readonly float Speed;

	public LoadingPurchase(float Speed, Building building, string Name, string description, Action action, int food, int wood, int gold)
        : base(Name, description, action, food, wood, gold)
    {
        this.Speed = Speed;
        this.building = building;
    }

    public override void Do()
    {
        if (building.CanStartTransaction() && PlayerState.Instance.Pay(food, wood, gold))
                building.AddTransaction(new Transaction(this, Speed));
    }

    public void InvokeAction()
    {
        action.Invoke();
    }

    public void Reset()
    {
        PlayerState.Instance.Pay(-food, -wood, -gold);
    }
}
