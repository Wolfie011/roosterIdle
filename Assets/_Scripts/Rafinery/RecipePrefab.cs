using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePrefab : MonoBehaviour
{
    public RafineryUI rafineryUI;
    public Producible producible;

    public Transform requiredItemsContent;
    public GameObject requiredPrefab;

    private void Awake()
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = producible.Icon;
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = producible.Name;
        gameObject.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = $"{producible.productionTime}";

        foreach(var item in producible.ItemsNeeded)
        {
            requiredPrefab requPrefab = Instantiate(requiredPrefab, requiredItemsContent).GetComponent<requiredPrefab>();
            requPrefab.amountNeeded = item.Value;
            requPrefab.requiredItem = item.Key;
        }

    }
    public void updateAmounts()
    {
        int childCount = requiredItemsContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(requiredItemsContent.GetChild(i).gameObject);
        }
        foreach (var item in producible.ItemsNeeded)
        {
            requiredPrefab requPrefab = Instantiate(requiredPrefab, requiredItemsContent).GetComponent<requiredPrefab>();
            requPrefab.amountNeeded = item.Value;
            requPrefab.requiredItem = item.Key;
        }
    }
    public void startSmelting()
    {
        /*int itemTest = 0;
        foreach(var item in producible.ItemsNeeded)
        {
            if (StorageManager.current.IsEnoughOf(item.Key, item.Value))
            {
                itemTest++;
            }
            else return;
        }
        if(itemTest == producible.ItemsNeeded.Count)
        {
            rafineryUI.addProductQue(producible);
        }*/

        foreach (var itemPair in producible.ItemsNeeded)
        {
            if (!StorageManager.current.IsEnoughOf(itemPair.Key, itemPair.Value))
            {
                Debug.Log("Not enough items");
                return;
            }
        }
        StorageManager.current.UpdateItems(producible.ItemsNeeded, false);
        rafineryUI.createNewProcess(producible);
        rafineryUI.updateReqAmounts();
    }
}
