using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producible : CollectibleItem
{
    public List<CollectibleItem> ItemTypes;
    public List<int> ItemsAmounts;

    public TimePeriod TimeStruct;

    public Dictionary<CollectibleItem, int> ItemsNeeded;
    public Dictionary<CollectibleItem, int> ItemsAquired;
    public TimeSpan productionTime;
    public int AquiredAmount;
    
    [System.Serializable]
    public struct TimePeriod
    {
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
    }

    protected void OnValidate()
    {
        ItemsNeeded = new Dictionary<CollectibleItem, int>();

        for (int i = 0; i < ItemTypes.Count && i < ItemsAmounts.Count; i++)
        {
            ItemsNeeded.Add(ItemTypes[i], ItemsAmounts[i]);
        }
        productionTime = new TimeSpan(TimeStruct.Days, TimeStruct.Hours, TimeStruct.Minutes, TimeStruct.Seconds);

        ItemsAquired = new Dictionary<CollectibleItem, int>()
        {
            {this, AquiredAmount}
        };
    }
}