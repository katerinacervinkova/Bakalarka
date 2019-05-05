public class MainBuilding : Building {

    private void Awake()
    {
        Name = "Main building";
    }

    protected override void InitPurchases()
    {
        Purchases.Add(new LoadingPurchase(1, this, "Unit", "Create a unit", () => owner.CreateUnit(this), food: 20, wood: 0, gold: 0));
    }
}
