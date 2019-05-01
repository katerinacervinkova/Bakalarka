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
    private Image toolTip;
    private Text toolTipText;
    [SerializeField]
    private List<PurchaseButton> buttons;
    [SerializeField]
    private List<Scheduler> schedulers;
    [SerializeField]
    private Text objectText;
    [SerializeField]
    private Text resourceText;
    [SerializeField]
    private GameObject target;


    private void Start()
    {
        toolTipText = toolTip.GetComponentInChildren<Text>();
    }

    public void ShowButtons(List<Purchase> transactions)
    {
        for (int i = 0; i < schedulers.Count; i++)
        {
            if (i < transactions.Count)
            {
                buttons[i].SetPurchase(transactions[i]);
                buttons[i].gameObject.SetActive(true);
            }
            else
                buttons[i].gameObject.SetActive(false);
        }
    }

    public void HideButtons()
    {
        buttons.ForEach(b => b.gameObject.SetActive(false));
    }

    public void ShowTransactions(List<Transaction> transactions)
    {
        for (int i = 0; i < schedulers.Count; i++)
        {
            if (i < transactions.Count)
            {
                schedulers[i].image.fillAmount = 1 - (transactions[i].Progress / transactions[i].MaxProgress);
                schedulers[i].gameObject.SetActive(true);
            }
            else
                schedulers[i].gameObject.SetActive(false);
        }
    }

    public void HideTransactions()
    {
        schedulers.ForEach(s => s.gameObject.SetActive(false));
    }

    public void OnClickScheduler(int index)
    {
        ((Building)PlayerState.Instance.SelectedObject).RemoveTransaction(index);
    }

    public void ChangeResourceText(string resourceString)
    {
        resourceText.text = resourceString;
    }

    public void ShowObjectText(string name, string description)
    {
        objectText.text = string.Format("<b><i>{0}</i></b>\n{1}", name, description);
        objectText.gameObject.SetActive(true);
    }

    public void HideObjectText()
    {
        objectText.gameObject.SetActive(false);
    }
    
    public void ShowToolTip(Vector3 position, string description)
    {
        toolTip.transform.position = position;
        toolTipText.text = description;
        toolTip.gameObject.SetActive(true);
    }
    public void HideToolTip()
    {
        toolTip.gameObject.SetActive(false);
    }

    public void ShowTarget(Vector3 destination)
    {
        target.transform.position = destination;
        target.SetActive(true);
    }

    public void HideTarget()
    {
        target?.SetActive(false);
    }
}