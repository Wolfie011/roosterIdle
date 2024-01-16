using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopUI : MonoBehaviour
{
    public Coop parentCoop;
    public CoopSlot slot;

    [SerializeField] public GameObject defView;
    [SerializeField] public GameObject dropView;
    [SerializeField] public GameObject innerStorageView;

    [SerializeField] public GameObject chickenView;
    [SerializeField] public Transform chickenContent;

    [SerializeField] public Transform dropContent;
    [SerializeField] public Transform innerStorageContent;

    public GameObject chickenPrefab;
    public GameObject DROPPrefab;
    public GameObject dropStoragePrefab;

    public Dictionary<CollectibleItem, int> storageCoop = new Dictionary<CollectibleItem, int>();

    public RectTransform rt;
    public RectTransform prt;
    public bool isVisible = false;

    private void Awake()
    {
        rt = gameObject.transform.parent.GetComponent<RectTransform>();
        prt = gameObject.transform.parent.GetComponentInParent<RectTransform>();
    }
    public void Initialize()
    {
        InitializeSlot();
        DefWindow_Click();
        //getItems();
    }
    public void getItems()
    {
        int childCount = innerStorageContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Product prodo = innerStorageContent.GetChild(i).gameObject.GetComponent<DropStorage>().drop;
            int value;
            if (storageCoop.TryGetValue(prodo, out value))
            {
                if(value > 0)
                {
                    Dictionary<CollectibleItem, int> temp = new Dictionary<CollectibleItem, int>()
                    {
                        { prodo, innerStorageContent.GetChild(i).gameObject.GetComponent<DropStorage>().amount }
                    };
                    StorageManager.current.UpdateItems(temp, true);
                    innerStorageContent.GetChild(i).gameObject.GetComponent<DropStorage>().amount = 0;
                    innerStorageContent.GetChild(i).gameObject.GetComponent<DropStorage>().updateVisual();
                }
            }
        }
    }
    public void InitializeSlot()
    {
        slot.coopUI = this;
        slot.CheckOnLoad();
    }
    //  (Screen.height + (Screen.height/2))
    public void openWindow()
    {
        if (!isVisible)
        {
            LeanTween.moveY(rt, 0, 0.2f);
            isVisible = true;
        }
    }
    public void closeWindow()
    {
        LeanTween.moveY(rt, (Screen.height), 0.2f);
        isVisible = false;
    }
    public void InitializeDrops()
    {
        if(slot.chicken == null)
        {

        }
        else
        {
            Chicken chicken = slot.chicken;

            int childCount = dropContent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(dropContent.GetChild(i).gameObject);
                Destroy(innerStorageContent.GetChild(i).gameObject);
            }

            foreach (var dropT in chicken.drop)
            {
                DropStorage dropStoragePrefabT = Instantiate(dropStoragePrefab, innerStorageContent).GetComponent<DropStorage>();
                dropStoragePrefabT.drop = dropT;

                Drop dropPrefab = Instantiate(DROPPrefab, dropContent).GetComponent<Drop>();
                dropPrefab.drop = dropT;
                dropPrefab.coopUI = this;
                dropPrefab.dropStorage = dropStoragePrefabT;
            }
        }
    }
    public void InitializeChickens(CoopSlot slot)
    {
        int childCount = chickenContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(chickenContent.GetChild(i).gameObject);
        }
        if (StorageManager.current.chickens == null)
        {
            Debug.Log("No chickens avalible");
        }
        else
        {
            foreach (var chickenT in StorageManager.current.chickens)
            {
                if (chickenT.Value.inUse)
                {

                }
                else
                {
                    ChickenSlot chickenHolder = Instantiate(chickenPrefab, chickenContent).GetComponent<ChickenSlot>();
                    chickenHolder.chicken = chickenT.Value;
                    chickenHolder.chickenKey = chickenT.Key;
                    chickenHolder.slot = slot;
                    chickenHolder.coopUI = this;
                }

            }
        }
        ChickenWindow_Click();
    }

    public void ChickenWindow_Click()
    {
        chickenView.SetActive(true);
        defView.SetActive(false);
    }
    public void DefWindow_Click()
    {
        chickenView.SetActive(false);
        defView.SetActive(true);
    }
}
