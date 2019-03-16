using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour {

    public List<Button> buttons;
    public PlayerState playerState;

    public void SetActive(List<Transaction> transactions, bool active)
    {
        if (active)
            for (int i = 0; i < transactions.Count; i++)
            {
                int k = i;
                buttons[k].onClick.AddListener(() => transactions[k].Do(playerState));
                buttons[k].gameObject.SetActive(true);
                buttons[k].GetComponentInChildren<Text>().text = transactions[k].name;
            }
        else
            buttons.ForEach(b => { b.onClick.RemoveAllListeners(); b.gameObject.SetActive(false); });
    }
}