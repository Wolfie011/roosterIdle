using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Egg", menuName = "GameObjects/StorageItems/Egg")]

public class Egg : CollectibleItem
{
    public string ID;
    public Dictionary<CollectibleItem, int> ItemsNeeded;
    public TimePeriod hatchTime;
    public TimeSpan productionTime;

    public bool inUse;

    public int Strength = 1;
    public int Growth = 1;
    public int Gain = 1;

    public Chicken hatchedChicken;
    //[SerializeField] public List<Product> drop;

    public Types eggType;

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
        ItemsNeeded = new Dictionary<CollectibleItem, int>() { { this, 1 } };
        productionTime = new TimeSpan(hatchTime.Days, hatchTime.Hours, hatchTime.Minutes, hatchTime.Seconds);
    }
}
public enum Types
{
    Default,
    c1,
    c2,
    c3,
    c4,
    c5,
    c6,
    c7,
    c8,
    c9,
    c10,
    c11,
    c12,
    c13,
    c14,
    c15,
    c16,
    c17,
    c18,
    c19,
    c20,
}
