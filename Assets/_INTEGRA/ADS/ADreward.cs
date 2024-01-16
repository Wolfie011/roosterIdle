using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "adReward", menuName = "GameObjects/adReward")]
public class ADreward : ScriptableObject
{
    public RewardType rewardType;
    public int amount;
    public CollectibleItem collectibleItem;

    public Egg egg;
    public List<int> paramS;
}
public enum RewardType
{
    Golds,
    Crystals,
    CollectibleItem,
    Egg
}
