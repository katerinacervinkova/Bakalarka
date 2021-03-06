﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPurchases : MonoBehaviour {

    public Player player;
    
    private Dictionary<PurchasesEnum, Purchase> purchases;
    private PlayerState PlayerState => PlayerState.Get(player.playerControllerId);

    // all purchases images
    #region images
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
    private Texture2D sawmillImage;
    [SerializeField]
    private Texture2D bankImage;
    [SerializeField]
    private Texture2D unitImage;
    [SerializeField]
    private Texture2D stoneAgeImage;
    [SerializeField]
    private Texture2D ironAgeImage;
    [SerializeField]
    private Texture2D diamondAgeImage;
    [SerializeField]
    private Texture2D books1Image;
    [SerializeField]
    private Texture2D books2Image;
    [SerializeField]
    private Texture2D books3Image;
    [SerializeField]
    private Texture2D books4Image;
    [SerializeField]
    private Texture2D books5Image;
    [SerializeField]
    private Texture2D buildingBooksImage;
    [SerializeField]
    private Texture2D medicineBooks1Image;
    [SerializeField]
    private Texture2D medicineBooks2Image;
    [SerializeField]
    private Texture2D intelligenceImage;
    [SerializeField]
    private Texture2D buildingImage;
    [SerializeField]
    private Texture2D healingImage;
    [SerializeField]
    private Texture2D gear1Image;
    [SerializeField]
    private Texture2D gear2Image;
    [SerializeField]
    private Texture2D gear3Image;
    [SerializeField]
    private Texture2D gear4Image;
    [SerializeField]
    private Texture2D gear5Image;
    [SerializeField]
    private Texture2D flourImage;
    [SerializeField]
    private Texture2D breadImage;
    #endregion


    private void Start() {

        purchases = new Dictionary<PurchasesEnum, Purchase>
        {
            #region unit purchases
            [PurchasesEnum.MainBuilding] = new Purchase(
                "Main building", player.playerControllerId, mainBuildingImage, "Create Main Building.",
                s => player.CreateTempBuilding(BuildingEnum.MainBuilding),
                s => true, food: 1000, wood: 1000, gold: 1000),

            [PurchasesEnum.Library] = new Purchase(
                "Library", player.playerControllerId, libraryImage, "Create Library, which increases units' intelligence.",
                s => player.CreateTempBuilding(BuildingEnum.Library),
                s => ReachedAge(PlayerState.AgeEnum.Stone), food: 0, wood: 1000, gold: 1200),

            [PurchasesEnum.Barracks] = new Purchase(
                "Barracks", player.playerControllerId, barracksImage, "Create Barracks, which increase units' swordsmanship.",
                s => player.CreateTempBuilding(BuildingEnum.Barracks),
                s => ReachedAge(PlayerState.AgeEnum.Stone), food: 0, wood: 1000, gold: 200),

            [PurchasesEnum.Infirmary] = new Purchase(
                "Infirmary", player.playerControllerId, infirmaryImage, "Create Infirmary, which increases units' health.",
                s => player.CreateTempBuilding(BuildingEnum.Infirmary),
                s => ReachedAge(PlayerState.AgeEnum.Iron), food: 100, wood: 500, gold: 500),

            [PurchasesEnum.House] = new Purchase(
                "House", player.playerControllerId, houseImage, "Create House, which increases maximum population.",
                s => player.CreateTempBuilding(BuildingEnum.House),
                s => true, food: 0, wood: 500, gold: 0),

            [PurchasesEnum.Mill] = new Purchase(
                "Mill", player.playerControllerId, millImage, "Create Mill to gather food.",
                s => player.CreateTempBuilding(BuildingEnum.Mill),
                s => true, food: 0, wood: 1000, gold: 500),

            [PurchasesEnum.Sawmill] = new Purchase(
                "Sawmill", player.playerControllerId, sawmillImage, "Create Sawmill to gather wood.",
                s => player.CreateTempBuilding(BuildingEnum.Sawmill),
                s => ReachedAge(PlayerState.AgeEnum.Stone), food: 500, wood: 500, gold: 500),

            [PurchasesEnum.Bank] = new Purchase(
                "Bank", player.playerControllerId, bankImage, "Create Bank to gather gold.",
                s => player.CreateTempBuilding(BuildingEnum.Bank),
                s => ReachedAge(PlayerState.AgeEnum.Iron), food: 1000, wood: 1500, gold: 300),
            #endregion

            #region main building purchases
            [PurchasesEnum.Unit] = new LoadingPurchase(
                0.5f, "Unit", player.playerControllerId, unitImage, "Create a unit",
                s => player.CreateUnit(s as Building),
                s => true, food: 150, wood: 50, gold: 20, population: 1),

            [PurchasesEnum.StoneAge] = new LoadingPurchase(
                0.1f, "Stone Age", player.playerControllerId, stoneAgeImage, "Advance to the Stone Age. Grants access to new advancements and foundations.",
                s => PlayerState.Age = PlayerState.AgeEnum.Stone,
                s => true, food: 2000, wood: 0, gold: 2000, oneTimePurchase: true),

            [PurchasesEnum.IronAge] = new LoadingPurchase(
                0.08f, "Iron Age", player.playerControllerId, ironAgeImage, "Advance to the Iron Age. Grants access to new advancements and foundations.",
                s => PlayerState.Age = PlayerState.AgeEnum.Iron,
                s => ReachedAge(PlayerState.AgeEnum.Stone), food: 5000, wood: 1000, gold: 3000, oneTimePurchase: true),

            [PurchasesEnum.DiamondAge] = new LoadingPurchase(
                0.06f, "Diamond Age", player.playerControllerId, diamondAgeImage, "Advance to the Diamond Age. Grants access to new advancements and foundations.",
                s => PlayerState.Age = PlayerState.AgeEnum.Diamond,
                s => ReachedAge(PlayerState.AgeEnum.Iron), food: 10000, wood: 2000, gold: 7000, oneTimePurchase: true),
            #endregion

            #region library purchases
            [PurchasesEnum.Books1] = new LoadingPurchase(
                0.3f, "Books 1", player.playerControllerId, books1Image, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 20; (s as Library).Books1 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Stone),
                food: 0, wood: 500, gold: 500, oneTimePurchase: true),

            [PurchasesEnum.Books2] = new LoadingPurchase(
                0.3f, "Books 2", player.playerControllerId, books2Image, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 35; (s as Library).Books2 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Library).Books1,
                food: 500, wood: 2000, gold: 2500, oneTimePurchase: true),

            [PurchasesEnum.Books3] = new LoadingPurchase(
                0.25f, "Books 3", player.playerControllerId, books3Image, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 55; (s as Library).Books3 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Iron) && PlayerState.Get(player.playerControllerId).units.Count > 30 && (s as Library).Books2,
                food: 1000, wood: 3000, gold: 3500, oneTimePurchase: true),

            [PurchasesEnum.Books4] = new LoadingPurchase(
                0.25f, "Books 4", player.playerControllerId, books4Image, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 75; (s as Library).Books4 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Diamond) && PlayerState.buildings.Where(b => b is Mill).Any() && (s as Library).Books3,
                food: 2000, wood: 5000, gold: 5000, oneTimePurchase: true),

            [PurchasesEnum.Books5] = new LoadingPurchase(
                0.2f, "Books 5", player.playerControllerId, books5Image, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 100; (s as Library).Books5 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Diamond) && (s as Library).Books4 && PlayerState.MedicineBooks2
                && PlayerState.buildings.Where(b => b is Barracks && (b as Barracks).Gear5).Any(),
                food: 3000, wood: 6000, gold: 7000, oneTimePurchase: true),

            [PurchasesEnum.BuildingBooks] = new LoadingPurchase(
                0.3f, "Building Books", player.playerControllerId, buildingBooksImage, "Supply library with new books about building. Enables building skill development in library.",
                s => PlayerState.BuildingBooks = true,
                s => ReachedAge(PlayerState.AgeEnum.Stone) && (s as Library).Books2 && PlayerState.buildings.Where(b => b is Sawmill).Any(),
                food: 500, wood: 1500, gold: 1500, oneTimePurchase: true),

            [PurchasesEnum.MedicineBooks1] = new LoadingPurchase(
                0.3f, "Medicine Books 1", player.playerControllerId, medicineBooks1Image, "Supply library with new books about medicine. Enables healing skill development in library.",
                s => PlayerState.MedicineBooks1 = true,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Library).Books3 && PlayerState.buildings.Where(b => b is Infirmary).Any(),
                food: 1500, wood: 500, gold: 1500, oneTimePurchase: true),

            [PurchasesEnum.MedicineBooks2] = new LoadingPurchase(
                0.25f, "Medicine Books 2", player.playerControllerId, medicineBooks2Image, "Supply library with new books about medicine. Increases doctors' effectivity.",
                s => PlayerState.MedicineBooks2 = true,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && PlayerState.MedicineBooks1,
                food: 2500, wood: 500, gold: 2500, oneTimePurchase: true),


            [PurchasesEnum.Intelligence] = new LoadingPurchase(
                5, "Intelligence", player.playerControllerId, intelligenceImage, "Change library focus to intelligence",
                s => (s as Library).Focus = Library.FocusEnum.Intelligence,
                s => ReachedAge(PlayerState.AgeEnum.Stone) && (s as Library).Focus != Library.FocusEnum.Intelligence,
                food: 100, wood: 0, gold: 0, oneTimePurchase: false),

            [PurchasesEnum.Building] = new LoadingPurchase(
                5, "Building", player.playerControllerId, buildingImage, "Change library focus to building",
                s => (s as Library).Focus = Library.FocusEnum.Building,
                s => ReachedAge(PlayerState.AgeEnum.Stone) && PlayerState.BuildingBooks && (s as Library).Focus != Library.FocusEnum.Building,
                food: 0, wood: 100, gold: 0, oneTimePurchase: false),

            [PurchasesEnum.Healing] = new LoadingPurchase(
                5, "Healing", player.playerControllerId, healingImage, "Change library focus to healing",
                s => (s as Library).Focus = Library.FocusEnum.Healing,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && PlayerState.MedicineBooks1 && (s as Library).Focus != Library.FocusEnum.Healing,
                food: 0, wood: 0, gold: 100, oneTimePurchase: false),
            #endregion

            #region barracks purchases
            [PurchasesEnum.Gear1] = new LoadingPurchase(
                0.3f, "Gear 1", player.playerControllerId, gear1Image, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => { (s as Barracks).maxSwordsmanship = 20; (s as Barracks).Gear1 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Stone),
                food: 200, wood: 500, gold: 500, oneTimePurchase: true),

            [PurchasesEnum.Gear2] = new LoadingPurchase(
                0.3f, "Gear 2", player.playerControllerId, gear2Image, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => { (s as Barracks).maxSwordsmanship = 35; (s as Barracks).Gear2 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Stone) && (s as Barracks).Gear1 && PlayerState.buildings.Where(b => b is Library && (b as Library).Books1).Any(),
                food: 300, wood: 700, gold: 700, oneTimePurchase: true),

            [PurchasesEnum.Gear3] = new LoadingPurchase(
                0.25f, "Gear 3", player.playerControllerId, gear3Image, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => { (s as Barracks).maxSwordsmanship = 55; (s as Barracks).Gear3 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Barracks).Gear2,
                food: 700, wood: 1000, gold: 1500, oneTimePurchase: true),

            [PurchasesEnum.Gear4] = new LoadingPurchase(
                0.25f, "Gear 4", player.playerControllerId, gear4Image, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => { (s as Barracks).maxSwordsmanship = 75; (s as Barracks).Gear4 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Barracks).Gear3 && PlayerState.buildings.Where(b => b is Library && (b as Library).Books2).Any(),
                food: 1000, wood: 1500, gold: 2500, oneTimePurchase: true),

            [PurchasesEnum.Gear5] = new LoadingPurchase(
                0.2f, "Gear 5", player.playerControllerId, gear5Image, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => { (s as Barracks).maxSwordsmanship = 100; (s as Barracks).Gear5 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Diamond) && (s as Barracks).Gear4 && PlayerState.buildings.Where(b => b is Library && (b as Library).Books4).Any(),
                food: 2000, wood: 3000, gold: 6000, oneTimePurchase: true),
            #endregion

            #region mill
            [PurchasesEnum.Flour] = new LoadingPurchase(
                0.3f, "Flour", player.playerControllerId, flourImage, "Supply mill with better gear. Increases gathering speed.",
                s => { (s as Mill).Speed *= 2; (s as Mill).Flour = true; },
                s => ReachedAge(PlayerState.AgeEnum.Stone),
                food: 500, wood: 1000, gold: 1500, oneTimePurchase: true),

            [PurchasesEnum.Bread] = new LoadingPurchase(
                0.3f, "Bread", player.playerControllerId, breadImage, "Supply mill with better gear. Increases gathering speed.",
                s => { (s as Mill).Speed *= 2; (s as Mill).Bread = true; },
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Mill).Flour,
                food: 1000, wood: 1500, gold: 2000, oneTimePurchase: true),
            #endregion
        };
    }

    public Purchase Get(PurchasesEnum purchasesEnum) => purchases[purchasesEnum];

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
            case BuildingEnum.Sawmill:
                return Get(PurchasesEnum.Sawmill);
            case BuildingEnum.Bank:
                return Get(PurchasesEnum.Bank);
            default:
                return null;
        }
    }

    private bool ReachedAge(PlayerState.AgeEnum age) => PlayerState.Age >= age;
}
