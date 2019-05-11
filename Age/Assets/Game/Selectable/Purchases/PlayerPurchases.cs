using System.Collections.Generic;
using UnityEngine;

public class PlayerPurchases : MonoBehaviour {

    public Player player;
    private Dictionary<PurchasesEnum, Purchase> purchases;

	private void Awake() {
        purchases = new Dictionary<PurchasesEnum, Purchase>
        {
            [PurchasesEnum.MainBuilding] = new Purchase("Main building", "Create Main Building", s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.MainBuilding), food: 0, wood: 50, gold: 0),
            [PurchasesEnum.Library] = new Purchase("Library", "Create Library, which increases units' intelligence", s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.Library), food: 0, wood: 20, gold: 10),
            [PurchasesEnum.Barracks] = new Purchase("Barracks", "Create Barracks, which increase units' swordsmanship", s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.Barracks), food: 0, wood: 20, gold: 20),
            [PurchasesEnum.Infirmary] = new Purchase("Infirmary", "Create Infirmary, which increases units' health", s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.Infirmary), food: 0, wood: 20, gold: 0),
            [PurchasesEnum.House] = new Purchase("House", "Create House, which increases maximum population", s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.House), food: 0, wood: 10, gold: 0),
            [PurchasesEnum.Unit] = new LoadingPurchase(1, "Unit", "Create a unit", s => PlayerState.Instance.player.CreateUnit(s as Building), food: 20, wood: 0, gold: 0, population: 1)
        };
    }

    public Purchase Get(PurchasesEnum purchasesEnum)
    {
        return purchases[purchasesEnum];
    }
}
