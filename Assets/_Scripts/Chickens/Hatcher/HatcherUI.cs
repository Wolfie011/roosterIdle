using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatcherUI : MonoBehaviour
{
    public Hatcher parentHatcher;
    public List<HatcherSlot> slots;

    [SerializeField] private GameObject hatcherView;
    [SerializeField] private GameObject eggView;

    [SerializeField] private Transform hatchContent;
    [SerializeField] private Transform eggContent;

    public GameObject eggPrefab;


    private RectTransform rt;
    private RectTransform prt;
    public bool isVisible = false;
    private void Awake()
    {
        rt = gameObject.transform.parent.GetComponent<RectTransform>();
        prt = gameObject.transform.parent.GetComponentInParent<RectTransform>();
    }
    public void Initialize()
    {
        InitializeSlots();
        HatcherWindow_Click();
    }
    public void InitializeSlots()
    {
        foreach (var slot in slots)
        {
            slot.hatcherUI = this;
            slot.CheckOnLoad();
        }
    }
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
    public void InitializeEggs(HatcherSlot slot)
    {
        int childCount = eggContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(eggContent.GetChild(i).gameObject);
        }

        if (StorageManager.current.eggs == null)
        {
            Debug.Log("No eggs avalible");
        }
        else
        {
            foreach (var egg in StorageManager.current.eggs)
            {
                if (egg.Value.inUse)
                {

                }
                else
                {
                    EggSlot eggHolder = Instantiate(eggPrefab, eggContent).GetComponent<EggSlot>();
                    eggHolder.egg = egg.Value;
                    eggHolder.eggKey = egg.Key;
                    eggHolder.slot = slot;
                    eggHolder.hatcherUI = this;
                }
            }
        }
        EggWindow_Click();
    }

    public void EggWindow_Click()
    {
        eggView.SetActive(true);
        hatcherView.SetActive(false);
    }
    public void HatcherWindow_Click()
    {
        hatcherView.SetActive(true);
        eggView.SetActive(false);
    }
}
