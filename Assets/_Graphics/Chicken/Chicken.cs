using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chicken", menuName = "GameObjects/StorageItems/Chicken")]
public class Chicken : CollectibleItem
{
    public string ID;
    public int Strength;
    public int Growth;
    public int Gain;

    [SerializeField] public Crop FeedType;
    public int FeedRequ = 10;
    public bool inUse = false;
    [SerializeField] public List<Product> drop;

    public Types chickenType;
    public TimePeriod breedTimePeriod;
    public TimeSpan breedTime;

    [System.Serializable]
    public struct TimePeriod
    {
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
    }

    public Dictionary<CollectibleItem, int> thatChicken;
    protected void OnValidate()
    {
        thatChicken = new Dictionary<CollectibleItem, int>() { { this, 1 } };
        breedTime = new TimeSpan(breedTimePeriod.Days, breedTimePeriod.Hours, 
            breedTimePeriod.Minutes, breedTimePeriod.Seconds);

    }
}

