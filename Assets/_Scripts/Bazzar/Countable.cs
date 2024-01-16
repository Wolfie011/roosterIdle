using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countable : MonoBehaviour
{
    public int amount;
    public TextMeshProUGUI amountVisual;
    public CollectibleItem parentItem;
    public BazzarItem parentBazarItem;

    private void Awake()
    {
        amount = 0;
        updateVisual();
    }
    public void adder()
    {
        if(amount <= StorageManager.current.GetAmount(parentItem))
        {
            if (amount >= 9)
            {
                amount += 10;
            }
            else amount += 1;
            updateVisual();
        }
    }
    public void removeer()
    {
        if(amount >= 0)
        {
            if (amount <= 10)
            {
                amount -= 1;
            }
            else amount -= 10;
            updateVisual();
        }
    }
    void updateVisual()
    {
        amountVisual.text = amount.ToString();
        parentBazarItem.btnAmount.text = (parentItem.marketPrice * amount).ToString();
    }
}
