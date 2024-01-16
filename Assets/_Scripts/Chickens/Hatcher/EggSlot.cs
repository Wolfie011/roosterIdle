using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EggSlot : MonoBehaviour
{
    public Egg egg;
    public string eggKey;
    public HatcherSlot slot;
    public HatcherUI hatcherUI;
    private void Awake()
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = egg.Icon;
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = egg.Name;
        gameObject.transform.Find("Stats")
            .GetComponent<TextMeshProUGUI>().text = $"{egg.Strength} / {egg.Gain} / {egg.Growth}";
    }
    public void SelectEgg()
    {
        slot.egg = egg;
        slot.eggKey = eggKey;

        if (StorageManager.current.eggs.TryGetValue(eggKey, out Egg originalEgg))
        {
            originalEgg.inUse = true;
            StorageManager.current.eggs[eggKey] = originalEgg;
        }

        slot.TimerInit();
        slot.CheckOnLoad();
        hatcherUI.HatcherWindow_Click();
    }
}
