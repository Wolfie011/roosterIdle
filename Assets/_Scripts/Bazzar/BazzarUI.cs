using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BazzarUI : MonoBehaviour
{
    public Bazzar parentBazzar;

    [SerializeField] private GameObject holderPrefab;
    [SerializeField] private List<Transform> tabs;

    public int page = 0;

    public void Initialize()
    {
        ClearView();
        InitializeTab();
        swapTabs();
    }
    public void ClearView()
    {
        foreach (var tab in tabs)
        {
            int childCount = tab.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(tab.GetChild(i).gameObject);
            }
        }
    }
    public void swapTabs()
    {
        foreach (var tab in tabs)
        {
            tab.gameObject.SetActive(false);
        }
        tabs[page].gameObject.SetActive(true);
    }

    public void InitializeTab()
    {
        switch (page)
        {
            case 0:
                foreach (var chicken in StorageManager.current.chickens)
                {
                    GameObject holder = Instantiate(holderPrefab, tabs[0]);
                    holder.GetComponent<BazzarItem>().item = chicken.Value;
                    holder.GetComponent<BazzarItem>().bazzarUI = this;
                }
                break;
            case 1:
                foreach (var product in StorageManager.current.products)
                {
                    GameObject holder = Instantiate(holderPrefab, tabs[1]);
                    holder.GetComponent<BazzarItem>().item = product.Key;
                    holder.GetComponent<BazzarItem>().bazzarUI = this;
                }
                break;
            case 2:
                foreach (var product in StorageManager.current.products)
                {
                    //GameObject holder = Instantiate(holderPrefab, tabs[2]);
                    //holder.GetComponent<SellItemCount>().product = product.Key;

                    //holder.GetComponent<SellItemCount>().bazaarUI = instance;
                }
                break;
            case 3:
                foreach (var crop in StorageManager.current.crops)
                {
                    GameObject holder = Instantiate(holderPrefab, tabs[3]);
                    holder.GetComponent<BazzarItem>().item = crop.Key;
                    holder.GetComponent<BazzarItem>().bazzarUI = this;
                }
                break;
        }
    }
    public void Swap(int numberSwap)
    {
        page = numberSwap;
        Initialize();
        swapTabs();
    }
}
