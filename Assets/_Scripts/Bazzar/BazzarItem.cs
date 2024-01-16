using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BazzarItem : MonoBehaviour
{
    public BazzarUI bazzarUI;
    public CollectibleItem item;
    public TextMeshProUGUI btnAmount;

    private void Awake()
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = item.Icon;
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Name;
        if(item is Chicken chickenItem)
        {
            gameObject.transform.Find("chickenStats").GetComponent<TextMeshProUGUI>().text =
                $"{chickenItem.Strength} / {chickenItem.Growth} / {chickenItem.Gain}";
            gameObject.transform.Find("chickenStats").gameObject.SetActive(true);

            if (chickenItem.inUse)
            {
                gameObject.transform.Find("sell").GetComponent<Button>().interactable = false;
                gameObject.transform.Find("block").gameObject.SetActive(true);
            }
        }
        else
        {
            gameObject.transform.Find("countable").GetComponent<Countable>().parentItem = item;
            gameObject.transform.Find("countable").GetComponent<Countable>().parentBazarItem = this;
            gameObject.transform.Find("countable").gameObject.SetActive(true);
        }
    }
    public void sellItemxAmount()
    {
        if (StorageManager.current.IsEnoughOf(item, gameObject.transform.Find("countable").GetComponent<Countable>().amount))
        {
            Dictionary<CollectibleItem, int> tempSell = new Dictionary<CollectibleItem, int>
            {
                { item, gameObject.transform.Find("countable").GetComponent<Countable>().amount }
            };

            GameManager.current.GetCoins(item.marketPrice *
                gameObject.transform.Find("countable").GetComponent<Countable>().amount);
            StorageManager.current.UpdateItems(tempSell, false);
        }
        else Debug.Log("Not enough of items");
    }
}
