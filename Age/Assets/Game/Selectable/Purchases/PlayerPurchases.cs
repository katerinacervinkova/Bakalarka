using System.Collections.Generic;
using UnityEngine;

public class PlayerPurchases : MonoBehaviour {

    public Player player;
    private Dictionary<PurchasesEnum, Purchase> purchases;

    [SerializeField]
    private Texture2D mainBuildingImage;
    [SerializeField]
    private Texture2D libraryImage;
    [SerializeField]
    private Texture2D barracksImage;
    [SerializeField]
    private Texture2D infirmaryImage;
    [SerializeField]
    private Texture2D houseImage;
    [SerializeField]
    private Texture2D millImage;
    [SerializeField]
    private Texture2D unitImage;

    private void Start() {
        purchases = new Dictionary<PurchasesEnum, Purchase>
        {
            [PurchasesEnum.MainBuilding] = new Purchase(
                "Main building", player.playerControllerId, mainBuildingImage, "Create Main Building.", 
                s => PlayerState.Get(player.playerControllerId).player.CreateTempBuilding(BuildingEnum.MainBuilding), 
                food: 0, wood: 50, gold: 0),
            [PurchasesEnum.Library] = new Purchase(
                "Library", player.playerControllerId, libraryImage, "Create Library, which increases units' intelligence.", 
                s => PlayerState.Get(player.playerControllerId).player.CreateTempBuilding(BuildingEnum.Library), 
                food: 0, wood: 20, gold: 10),
            [PurchasesEnum.Barracks] = new Purchase(
                "Barracks", player.playerControllerId, barracksImage, "Create Barracks, which increase units' swordsmanship.", 
                s => PlayerState.Get(player.playerControllerId).player.CreateTempBuilding(BuildingEnum.Barracks), 
                food: 0, wood: 20, gold: 20),
            [PurchasesEnum.Infirmary] = new Purchase(
                "Infirmary", player.playerControllerId, infirmaryImage, "Create Infirmary, which increases units' health.", 
                s => PlayerState.Get(player.playerControllerId).player.CreateTempBuilding(BuildingEnum.Infirmary), 
                food: 0, wood: 20, gold: 0),
            [PurchasesEnum.House] = new Purchase(
                "House", player.playerControllerId, houseImage, "Create House, which increases maximum population.", 
                s => PlayerState.Get(player.playerControllerId).player.CreateTempBuilding(BuildingEnum.House), 
                food: 0, wood: 10, gold: 0),
            [PurchasesEnum.Mill] = new Purchase(
                "Mill", player.playerControllerId, millImage, "Create Mill to gather food.",
                s => PlayerState.Get(player.playerControllerId).player.CreateTempBuilding(BuildingEnum.Mill),
                food: 0, wood: 10, gold: 0),
            [PurchasesEnum.Unit] = new LoadingPurchase(
                1, "Unit", player.playerControllerId, unitImage, "Create a unit", 
                s => PlayerState.Get(player.playerControllerId).player.CreateUnit(s as Building), 
                food: 20, wood: 0, gold: 0, population: 1)
        };
    }

    public Purchase Get(PurchasesEnum purchasesEnum)
    {
        return purchases[purchasesEnum];
    }

    public Purchase Get(BuildingEnum buildingType)
    {
        switch (buildingType)
        {
            case BuildingEnum.MainBuilding:
                return Get(PurchasesEnum.MainBuilding);
            case BuildingEnum.Library:
                return Get(PurchasesEnum.Library);
            case BuildingEnum.Barracks:
                return Get(PurchasesEnum.Barracks);
            case BuildingEnum.Infirmary:
                return Get(PurchasesEnum.Infirmary);
            case BuildingEnum.House:
                return Get(PurchasesEnum.House);
            case BuildingEnum.Mill:
                return Get(PurchasesEnum.Mill);
            default:
                return null;
        }
    }
}
