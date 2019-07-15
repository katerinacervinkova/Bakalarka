using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UIManager>();
            return instance;
        }
    }

    [SerializeField]
    private GameObject healthBarsContainer;
    [SerializeField]
    private Image toolTip;
    [SerializeField]
    private Text toolTipText;
    [SerializeField]
    private List<PurchaseButton> buttons;
    [SerializeField]
    private List<Scheduler> schedulers;
    [SerializeField]
    private Text objectText;
    [SerializeField]
    private Text playerStateText;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private Button unitsButton;
    [SerializeField]
    private Button destroyButton;
    [SerializeField]
    private BuildingWindow buildingWindow;
    [SerializeField]
    private HealthBar healthBarPrefab;

    /// <summary>
    /// Building for which the building window is shown. Null if no building window is shown. 
    /// </summary>
    public Building BuildingWindowShown { get; private set; }

    /// <summary>
    /// Shows buttons with all available purchases for given selectable.
    /// </summary>
    public void ShowPurchaseButtons(List<Purchase> purchases, Selectable selectable)
    {
        int j = 0;
        for (int i = 0; i < schedulers.Count; i++)
        {
            while (j < purchases.Count && !purchases[j].IsActive(selectable))
                j++;
            buttons[i].gameObject.SetActive(j < purchases.Count);
            if (j < purchases.Count)
                buttons[i].SetPurchase(purchases[j++]);
        }
    }

    /// <summary>
    /// Hides all the purchase buttons.
    /// </summary>
    public void HidePurchaseButtons()
    {
        buttons.ForEach(b => { if (b != null) b.gameObject.SetActive(false); });
    }

    /// <summary>
    /// Shows the progress of all transactions the currently selected building.
    /// </summary>
    public void ShowTransactions(List<Transaction> transactions)
    {
        for (int i = 0; i < schedulers.Count; i++)
        {
            if (i < transactions.Count)
            {
                schedulers[i].toolTippedObject.Description = transactions[i].purchase.Name;
                schedulers[i].image.fillAmount = 1 - (transactions[i].Progress / transactions[i].MaxProgress);
                schedulers[i].gameObject.SetActive(true);
            }
            else
                schedulers[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Hides all transactions.
    /// </summary>
    public void HideTransactions()
    {
        schedulers.ForEach(s => { if (s != null) s.gameObject.SetActive(false); });
    }

    /// <summary>
    /// Creates a new health bar for given selectable. 
    /// </summary>
    /// <param name="offset">how far above the selectable the health bar should be</param>
    /// <returns>the health bar</returns>
    public HealthBar CreateHealthBar(Selectable selectable, float offset)
    {
        HealthBar healthBar = Instantiate(healthBarPrefab, healthBarsContainer.transform);
        healthBar.positionOffset = offset;
        healthBar.selectable = selectable;
        return healthBar;
    }

    /// <summary>
    /// Shows the button for showing a building window.
    /// </summary>
    public void ShowBuildingWindowButton()
    {
        unitsButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the button for showing a building window.
    /// </summary>
    public void HideBuildingWindowButton()
    {
        if (unitsButton != null)
            unitsButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the button for destroying selected object.
    /// </summary>
    public void ShowDestroyButton()
    {
        destroyButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the button for destroying selected object.
    /// </summary>
    public void HideDestroyButton()
    {
        if (destroyButton != null)
            destroyButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Removes the clicked transaction.
    /// </summary>
    /// <param name="index">index of the transaction</param>
    public void OnClickTransaction(int index)
    {
        ((Building)PlayerState.Get().SelectedObject).RemoveTransaction(index);
    }

    /// <summary>
    /// Shows the building window for the selected building.
    /// </summary>
    public void OnClickBuildingWindow()
    {
        ((Building)PlayerState.Get().SelectedObject).ShowUnitsWindow();
    }

    /// <summary>
    /// Destroys the selected object.
    /// </summary>
    public void OnClickDestroyButton()
    {
        HideToolTip();
        PlayerState.Get().SelectedObject.owner.DestroySelectedObject(PlayerState.Get().SelectedObject);
    }

    /// <summary>
    /// Selects a random unit which has nothing to do.
    /// </summary>
    public void OnClickIdleButton()
    {
        PlayerState.Get().SelectIdle();
    }

    /// <summary>
    /// Shows the name and further information about the player.
    /// </summary>
    public void ChangePlayerStateText(string playerName, PlayerState.AgeEnum age, string description)
    {
        if (playerStateText != null)
            playerStateText.text = $"<b><i>{playerName}</i></b>\n<i>{age} Age</i>\n{description}";
    }

    /// <summary>
    /// Shows the name and further information about the selected object.
    /// </summary>
    public void ShowObjectText(string objectName, string description)
    {
        objectText.text = $"<b><i>{objectName}</i></b>\n{description}";
        objectText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the information about the selected object.
    /// </summary>
    public void HideObjectText()
    {
        if (objectText != null)
            objectText.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Shows the tooltip in the given position with given description.
    /// </summary>
    public void ShowToolTip(Vector3 position, string description)
    {
        toolTip.transform.position = position;
        toolTipText.text = description;
        toolTip.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    public void HideToolTip()
    {
        toolTip.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the target in the given position.
    /// </summary>
    public void ShowTarget(Vector3 destination)
    {
        target.transform.position = destination;
        target.SetActive(true);
    }

    /// <summary>
    /// Hides the target.
    /// </summary>
    public void HideTarget()
    {
        if (target != null)
            target.SetActive(false);
    }


    /// <summary>
    /// Shows the building window for the given building with the given list of units.
    /// </summary>
    /// <param name="action">action to be performed for the unit when the action button in its row is clicked</param>
    public void ShowBuildingWindow(Building building, List<Unit> units, Action<Unit> action = null, string actionName = null)
    {
        buildingWindow.Show(building, units, action, actionName);
        BuildingWindowShown = building;
    }

    /// <summary>
    /// Hides the building window.
    /// </summary>
    public void HideBuildingWindow()
    {
        buildingWindow.Hide();
        BuildingWindowShown = null;
    }

    /// <summary>
    /// Updates the description of all units in the building window.
    /// </summary>
    public void UpdateBuildingWindowDescriptions()
    {
        buildingWindow.UpdateDescriptions();
    }
}