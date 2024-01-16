using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class requiredPrefab : MonoBehaviour
{
    public CollectibleItem requiredItem;
    public int amountNeeded;

    private void Start()
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = requiredItem.Icon;
        gameObject.transform.Find("havereq").GetComponent<TextMeshProUGUI>().text = $"{StorageManager.current.GetAmount(requiredItem)} / {amountNeeded}";
        if(StorageManager.current.IsEnoughOf(requiredItem, amountNeeded)) gameObject.transform.Find("check").gameObject.SetActive(true);
        else gameObject.transform.Find("check").gameObject.SetActive(false);
    }
}
