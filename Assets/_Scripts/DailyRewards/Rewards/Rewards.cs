using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rewards", menuName = "GameObjects/Rewards")]
public class Rewards : ScriptableObject
{
    public Sprite Icon;
    public int Day;
    public int amount;
    public Chest chest;

    public RewardTypes rewardTypes;
}
public enum RewardTypes
{
    Coin,
    GoldChest,
    PlatinumChest,
    Gem
}
