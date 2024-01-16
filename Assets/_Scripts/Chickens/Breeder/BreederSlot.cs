using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreederSlot : MonoBehaviour
{
    public BreederUI breederUI;
    public int index;
    public Chicken chicken;
    public string chickenKey;

    private void Awake()
    {
        CheckOnLoad();
    }
    public void EggSelection(BreederSlot thisSlot)
    {
        breederUI.InitializeEggs(thisSlot);
    }
    public void removeChicken()
    {
        if (StorageManager.current.chickens.TryGetValue(chickenKey, out Chicken originalChicken))
        {
            originalChicken.inUse = false;
            StorageManager.current.chickens[chickenKey] = originalChicken;
        }
        else
        {
            Debug.Log("Key not found in dictionary.");
        }

        chicken = null;
        chickenKey = null;
        breederUI.parentBreeder.dataBRD.slotAssetName[index] = null;
        breederUI.checkChickens();
        CheckOnLoad();
    }
    public void CheckOnLoad()
    {
        if (chicken != null)
        {
            gameObject.transform.Find("selectChicken").gameObject.SetActive(false);
            gameObject.transform.Find("chickenSlot").gameObject.SetActive(true);

            Transform chickenT = gameObject.transform.Find("chickenSlot");
            chickenT.Find("Name").GetComponent<TextMeshProUGUI>().text = chicken.Name;
            chickenT.Find("Stats").GetComponent<TextMeshProUGUI>().text = $"S: {chicken.Strength} / G: {chicken.Growth} / G: {chicken.Gain}";
            chickenT.Find("Icon").GetComponent<Image>().sprite = chicken.Icon;
        }
        else
        {
            gameObject.transform.Find("selectChicken").gameObject.SetActive(true);
            gameObject.transform.Find("chickenSlot").gameObject.SetActive(false);
        }
    }
    public void InitializeOnLoad(string slotAssetName)
    {
        if(!string.IsNullOrEmpty(slotAssetName))
        {
            //Debug.Log("CHICKEN FOUND IN BARN: " + slotAssetName);
            chicken = StorageManager.current.chickens[slotAssetName];
            chickenKey = slotAssetName;
        }

        CheckOnLoad();
    }
}
