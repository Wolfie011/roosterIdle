using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem current;
    //all player's treasures
    public Dictionary<CurrencyType, int> CurrencyAmounts = new Dictionary<CurrencyType, int>();

    //currency texts
    [SerializeField] private List<GameObject> texts;

    //currency texts in a dictionary (for easier access)
    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts =
        new Dictionary<CurrencyType, TextMeshProUGUI>();

    [ReadOnly] public CurrencyData data = new CurrencyData();


    private void Awake()
    {
        current = this;

        //initialize dictionaries
        for (int i = 0; i < texts.Count; i++)
        {
            CurrencyAmounts.Add((CurrencyType)i, 0);
            currencyTexts.Add((CurrencyType)i, texts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        }
    }

    private void Start()
    {
        UpdateUI();
        //add listeners for currency change events and not enough currency
        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(OnCurrencyChange);
        EventManager.Instance.AddListener<NotEnoughCurrencyGameEvent>(OnNotEnough);
    }

    public void UpdateUI()
    {
        //set new currency amounts
        for (int i = 0; i < texts.Count; i++)
        {
            currencyTexts[(CurrencyType) i].text = CurrencyAmounts[(CurrencyType) i].ToString();
        }
    }
    
    private void OnCurrencyChange(CurrencyChangeGameEvent info)
    {
        //if the player's trying to spend currency
        if (info.amount < 0)
        {
            if (CurrencyAmounts[info.currencyType] < Math.Abs(info.amount))
            {
                EventManager.Instance.QueueEvent(new NotEnoughCurrencyGameEvent(info.amount, info.currencyType));
                return;
            }

            EventManager.Instance.QueueEvent(new EnoughCurrencyGameEvent());
        }
        
        //change currency amount
        CurrencyAmounts[info.currencyType] += info.amount;
        //update currency texts
        UpdateUI();
    }

    private void OnNotEnough(NotEnoughCurrencyGameEvent info)
    {
        //display that the player doesn't have any currency
        Debug.Log($"You don't have enough of {info.amount} {info.currencyType}");
    }
    private void OnApplicationQuit()
    {
        data.coins = CurrencyAmounts[CurrencyType.Coins];
        data.crystals = CurrencyAmounts[CurrencyType.Crystals];
        GameManager.current.saveData.AddData(data);
    }
}

public enum CurrencyType
{
    Coins,
    Crystals
}
