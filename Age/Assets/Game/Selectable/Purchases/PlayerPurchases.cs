using System;
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

    private void Awake() {
        purchases = new Dictionary<PurchasesEnum, Purchase>
        {
            [PurchasesEnum.MainBuilding] = new Purchase(
                "Main building", mainBuildingImage, "Create Main Building.", 
                s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.MainBuilding), 
                food: 0, wood: 50, gold: 0),
            [PurchasesEnum.Library] = new Purchase(
                "Library", libraryImage, "Create Library, which increases units' intelligence.", 
                s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.Library), 
                food: 0, wood: 20, gold: 10),
            [PurchasesEnum.Barracks] = new Purchase(
                "Barracks", barracksImage, "Create Barracks, which increase units' swordsmanship.", 
                s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.Barracks), 
                food: 0, wood: 20, gold: 20),
            [PurchasesEnum.Infirmary] = new Purchase(
                "Infirmary", infirmaryImage, "Create Infirmary, which increases units' health.", 
                s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.Infirmary), 
                food: 0, wood: 20, gold: 0),
            [PurchasesEnum.House] = new Purchase(
                "House", houseImage, "Create House, which increases maximum population.", 
                s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.House), 
                food: 0, wood: 10, gold: 0),
            [PurchasesEnum.Mill] = new Purchase(
                "House", millImage, "Create Mill to gather food.",
                s => PlayerState.Instance.player.CreateTempBuilding(BuildingEnum.Mill),
                food: 0, wood: 10, gold: 0),
            [PurchasesEnum.Unit] = new LoadingPurchase(
                1, "Unit", unitImage, "Create a unit", 
                s => PlayerState.Instance.player.CreateUnit(s as Building), 
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
