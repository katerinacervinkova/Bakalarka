using System.Collections.Generic;
using System.Linq;
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

    private PlayerState PlayerState => PlayerState.Get(player.playerControllerId);

    private void Start() {

        purchases = new Dictionary<PurchasesEnum, Purchase>
        {
            #region unit purchases
            [PurchasesEnum.MainBuilding] = new Purchase(
                "Main building", player.playerControllerId, mainBuildingImage, "Create Main Building.",
                s => player.CreateTempBuilding(BuildingEnum.MainBuilding),
                s => true, food: 0, wood: 50, gold: 0),

            [PurchasesEnum.Library] = new Purchase(
                "Library", player.playerControllerId, libraryImage, "Create Library, which increases units' intelligence.",
                s => player.CreateTempBuilding(BuildingEnum.Library),
                s => ReachedAge(PlayerState.AgeEnum.Stone), food: 0, wood: 20, gold: 10),

            [PurchasesEnum.Barracks] = new Purchase(
                "Barracks", player.playerControllerId, barracksImage, "Create Barracks, which increase units' swordsmanship.",
                s => player.CreateTempBuilding(BuildingEnum.Barracks),
                s => ReachedAge(PlayerState.AgeEnum.Stone), food: 0, wood: 20, gold: 20),

            [PurchasesEnum.Infirmary] = new Purchase(
                "Infirmary", player.playerControllerId, infirmaryImage, "Create Infirmary, which increases units' health.",
                s => player.CreateTempBuilding(BuildingEnum.Infirmary),
                s => ReachedAge(PlayerState.AgeEnum.Iron), food: 0, wood: 20, gold: 0),

            [PurchasesEnum.House] = new Purchase(
                "House", player.playerControllerId, houseImage, "Create House, which increases maximum population.",
                s => player.CreateTempBuilding(BuildingEnum.House),
                s => true, food: 0, wood: 10, gold: 0),

            [PurchasesEnum.Mill] = new Purchase(
                "Mill", player.playerControllerId, millImage, "Create Mill to gather food.",
                s => player.CreateTempBuilding(BuildingEnum.Mill),
                s => true, food: 0, wood: 10, gold: 0),
            #endregion

            #region main building purchases
            [PurchasesEnum.Unit] = new LoadingPurchase(
                1, "Unit", player.playerControllerId, unitImage, "Create a unit",
                s => player.CreateUnit(s as Building),
                s => true, food: 20, wood: 0, gold: 0, population: 1),

            [PurchasesEnum.StoneAge] = new LoadingPurchase(
                1, "Stone Age", player.playerControllerId, null, "Advance to the Stone Age. Grants access to new advancements and foundations.",
                s => PlayerState.Age = PlayerState.AgeEnum.Stone,
                s => true, food: 0, wood: 1, gold: 0, oneTimePurchase: true),

            [PurchasesEnum.IronAge] = new LoadingPurchase(
                1, "Iron Age", player.playerControllerId, null, "Advance to the Iron Age. Grants access to new advancements and foundations.",
                s => PlayerState.Age = PlayerState.AgeEnum.Iron,
                s => ReachedAge(PlayerState.AgeEnum.Stone), food: 0, wood: 1, gold: 0, oneTimePurchase: true),

            [PurchasesEnum.DiamondAge] = new LoadingPurchase(
                1, "Diamond Age", player.playerControllerId, null, "Advance to the Diamond Age. Grants access to new advancements and foundations.",
                s => PlayerState.Age = PlayerState.AgeEnum.Diamond,
                s => ReachedAge(PlayerState.AgeEnum.Iron), food: 0, wood: 1, gold: 0, oneTimePurchase: true),
            #endregion

            #region library purchases
            [PurchasesEnum.Books1] = new LoadingPurchase(
                1, "Books 1", player.playerControllerId, null, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 20; (s as Library).Books1 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Stone),
                food: 0, wood: 1, gold: 1, oneTimePurchase: true),

            [PurchasesEnum.Books2] = new LoadingPurchase(
                1, "Books 2", player.playerControllerId, null, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 35; (s as Library).Books2 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Library).Books1,
                food: 0, wood: 1, gold: 1, oneTimePurchase: true),

            [PurchasesEnum.Books3] = new LoadingPurchase(
                1, "Books 3", player.playerControllerId, null, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 55; (s as Library).Books3 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Iron) && PlayerState.Get(player.playerControllerId).units.Count > 30 && (s as Library).Books2,
                food: 0, wood: 1, gold: 1, oneTimePurchase: true),

            [PurchasesEnum.Books4] = new LoadingPurchase(
                1, "Books 4", player.playerControllerId, null, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 75; (s as Library).Books4 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Diamond) && PlayerState.buildings.Where(b => b is Mill).Any() && (s as Library).Books3,
                food: 0, wood: 1, gold: 1, oneTimePurchase: true),

            [PurchasesEnum.Books5] = new LoadingPurchase(
                1, "Books 5", player.playerControllerId, null, "Supply library with new books. Increases maximum intelligence.",
                s => { (s as Library).maxIntelligence = 100; (s as Library).Books5 = true; },
                s => ReachedAge(PlayerState.AgeEnum.Diamond) && (s as Library).Books4 && PlayerState.MedicineBooks2
                && PlayerState.buildings.Where(b => b is Barracks && (b as Barracks).Gear5).Any(),
                food: 0, wood: 10, gold: 10, oneTimePurchase: true),

            [PurchasesEnum.BuildingBooks] = new LoadingPurchase(
                1, "Building Books", player.playerControllerId, null, "Supply library with new books about building. Enables building skill development in library.",
                s => PlayerState.BuildingBooks = true,
                s => ReachedAge(PlayerState.AgeEnum.Stone) && (s as Library).Books2,
                food: 0, wood: 10, gold: 10, oneTimePurchase: true),

            [PurchasesEnum.MedicineBooks1] = new LoadingPurchase(
                1, "Medicine Books 1", player.playerControllerId, null, "Supply library with new books about medicine. Enables healing skill development in library.",
                s => PlayerState.MedicineBooks1 = true,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Library).Books3 && PlayerState.buildings.Where(b => b is Infirmary).Any(),
                food: 0, wood: 10, gold: 10, oneTimePurchase: true),

            [PurchasesEnum.MedicineBooks2] = new LoadingPurchase(
                1, "Medicine Books 2", player.playerControllerId, null, "Supply library with new books about medicine. Increases doctors' effectivity.",
                s => PlayerState.MedicineBooks2 = true,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && PlayerState.MedicineBooks1,
                food: 0, wood: 10, gold: 10, oneTimePurchase: true),


            [PurchasesEnum.Intelligence] = new LoadingPurchase(
                5, "Intelligence", player.playerControllerId, null, "Change library focus to intelligence",
                s => (s as Library).Focus = Library.FocusEnum.Intelligence,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Library).Focus != Library.FocusEnum.Intelligence,
                food: 0, wood: 0, gold: 0, oneTimePurchase: false),

            [PurchasesEnum.Building] = new LoadingPurchase(
                5, "Building", player.playerControllerId, null, "Change library focus to building",
                s => (s as Library).Focus = Library.FocusEnum.Building,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && PlayerState.BuildingBooks && (s as Library).Focus != Library.FocusEnum.Building,
                food: 0, wood: 0, gold: 0, oneTimePurchase: false),

            [PurchasesEnum.Healing] = new LoadingPurchase(
                5, "Healing", player.playerControllerId, null, "Change library focus to healing",
                s => (s as Library).Focus = Library.FocusEnum.Healing,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && PlayerState.MedicineBooks1 && (s as Library).Focus != Library.FocusEnum.Healing,
                food: 0, wood: 0, gold: 0, oneTimePurchase: false),
            #endregion

            #region barracks purchases
            [PurchasesEnum.Gear1] = new LoadingPurchase(
                1, "Books 1", player.playerControllerId, null, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => (s as Barracks).maxSwordsmanship = 20,
                s => ReachedAge(PlayerState.AgeEnum.Stone),
                food: 0, wood: 10, gold: 10, oneTimePurchase: true),

            [PurchasesEnum.Gear2] = new LoadingPurchase(
                1, "Books 2", player.playerControllerId, null, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => (s as Barracks).maxSwordsmanship = 35,
                s => ReachedAge(PlayerState.AgeEnum.Stone) && (s as Barracks).Gear1 && PlayerState.buildings.Where(b => b is Library && (b as Library).Books1).Any(),
                food: 0, wood: 10, gold: 10, oneTimePurchase: true),

            [PurchasesEnum.Gear3] = new LoadingPurchase(
                1, "Books 3", player.playerControllerId, null, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => (s as Barracks).maxSwordsmanship = 55,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Barracks).Gear2,
                food: 0, wood: 10, gold: 10, oneTimePurchase: true),

            [PurchasesEnum.Gear4] = new LoadingPurchase(
                1, "Books 4", player.playerControllerId, null, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => (s as Barracks).maxSwordsmanship = 75,
                s => ReachedAge(PlayerState.AgeEnum.Iron) && (s as Barracks).Gear3 && PlayerState.buildings.Where(b => b is Library && (b as Library).Books2).Any(),
                food: 0, wood: 10, gold: 0, oneTimePurchase: true),

            [PurchasesEnum.Gear5] = new LoadingPurchase(
                1, "Books 5", player.playerControllerId, null, "Supply barracks with better gear. Increases maximum swordsmanship.",
                s => (s as Barracks).maxSwordsmanship = 100,
                s => ReachedAge(PlayerState.AgeEnum.Diamond) && (s as Barracks).Gear4 && PlayerState.buildings.Where(b => b is Library && (b as Library).Books4).Any(),
                food: 0, wood: 10, gold: 0, oneTimePurchase: true),
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
            default:
                return null;
        }
    }

    private bool ReachedAge(PlayerState.AgeEnum age) => PlayerState.Age >= age;
}
