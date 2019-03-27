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
    private List<Button> buttons;

    private void Start()
    {
        toolTipText = toolTip.GetComponentInChildren<Text>();
    }

    public void SetActive(List<Transaction> transactions, bool active)
    {
        if (active)
            for (int i = 0; i < transactions.Count; i++)
            {
                int k = i;
                buttons[k].onClick.AddListener(() => transactions[k].Do());
                buttons[k].gameObject.SetActive(true);
                buttons[k].GetComponentInChildren<Text>().text = transactions[k].Name;
                buttons[k].GetComponent<ToolTipButton>().transaction = transactions[k];
            }
        else
            buttons.ForEach(b => { b.onClick.RemoveAllListeners(); b.gameObject.SetActive(false); });
    }
    
    public void ShowToolTip(Vector3 position, string description)
    {
        toolTip.transform.position = position;
        toolTip.gameObject.SetActive(true);
        toolTipText.text = description;
    }
    public void HideToolTip()
    {
        toolTip.gameObject.SetActive(false);
    }
}