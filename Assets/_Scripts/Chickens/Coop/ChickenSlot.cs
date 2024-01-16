using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChickenSlot : MonoBehaviour
{
    public CoopUI coopUI;
    public Chicken chicken;
    public string chickenKey;
    public CoopSlot slot;

    private void Awake()
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = chicken.Icon;
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = chicken.Name;
        gameObject.transform.Find("Stats")
            .GetComponent<TextMeshProUGUI>().text = $"{chicken.Strength} / {chicken.Gain} / {chicken.Growth}";
    }
    public void SelectChicken()
    {
        slot.chicken = chicken;
        slot.chickenKey = chickenKey;

        if (StorageManager.current.chickens.TryGetValue(chickenKey, out Chicken originalChicken))
        {
            originalChicken.inUse = true;
            StorageManager.current.chickens[chickenKey] = originalChicken;
        }
        else
        {
            Debug.Log("Key not found in dictionary.");
        }

        coopUI.Initialize();
        coopUI.dropView.gameObject.SetActive(true);
        coopUI.InitializeDrops();
    }
}
