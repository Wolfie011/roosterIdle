using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoopSlot : MonoBehaviour
{
    public CoopUI coopUI;

    public Chicken chicken;
    public string chickenKey;

    private void Awake()
    {
        CheckOnLoad();
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
        coopUI.parentCoop.dataCOP.assetID = null;
        coopUI.dropView.gameObject.SetActive(false);
        coopUI.innerStorageView.gameObject.SetActive(false);

        for(int i = 0; i < coopUI.innerStorageContent.childCount; i++)
        {
            coopUI.innerStorageContent.GetChild(i).gameObject.GetComponent<DropStorage>().removedAdder();
        }

        CheckOnLoad();
    }
    public void CheckOnLoad()
    {
        if(chicken != null)
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
    public void ChickenSelection(CoopSlot thisSlot)
    {
        coopUI.InitializeChickens(thisSlot);
        coopUI.innerStorageView.gameObject.SetActive(true);

    }
    public void InitializeOnLoad(string slotAssetID)
    {
        if (!string.IsNullOrEmpty(slotAssetID))
        {
            //Debug.Log("CHICKEN FOUND IN BARN: " + slotAssetName);
            chicken = StorageManager.current.chickens[slotAssetID];
            chickenKey = slotAssetID;

            coopUI.InitializeDrops();
            coopUI.dropView.gameObject.SetActive(true);
            coopUI.innerStorageView.gameObject.SetActive(true);

        }

        CheckOnLoad();
    }
}
