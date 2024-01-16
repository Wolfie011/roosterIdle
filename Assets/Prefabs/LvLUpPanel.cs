using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvLUpPanel : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform unlockContent;

    public List<ShopItem> shopItemList;
    private int test;
    private int test2;
    private void Awake()
    {
        test = LevelSystem.Level;
        test2 = test - 1;
        gameObject.transform.Find("FROM2").GetComponent<TextMeshProUGUI>().text = $"{test2} ===> {test}";
        foreach(var item in shopItemList)
        {
            if(item.Level == test)
            {
                GameObject itemHold = Instantiate(ItemPrefab, unlockContent);
                itemHold.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.Icon;
                itemHold.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Name;
            }
        }
    }

    public void close()
    {
        //clear
        Destroy(gameObject);
        //collect?
    }
}
