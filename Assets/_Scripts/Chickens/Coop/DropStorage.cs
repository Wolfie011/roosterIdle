using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropStorage : MonoBehaviour
{
    public Product drop;
    public int amount = 0;
    private void Start()
    {
        gameObject.transform.Find("DropStorage").transform.Find("Icon").GetComponent<Image>().sprite = drop.Icon;
        gameObject.transform.Find("DropStorage").transform.Find("Name").GetComponent<TextMeshProUGUI>().text = drop.Name;
        gameObject.transform.Find("DropStorage").transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = $"{amount}";
    }
    public void updateVisual()
    {
        amount = amount + drop.AquiredAmount;
        gameObject.transform.Find("DropStorage").transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = $"{amount}";
    }
    public void removedAdder()
    {
        Dictionary<CollectibleItem, int> tempDict = new Dictionary<CollectibleItem, int>
        {
            { drop, amount }
        };
        StorageManager.current.UpdateItems(tempDict, true);
    }
}
