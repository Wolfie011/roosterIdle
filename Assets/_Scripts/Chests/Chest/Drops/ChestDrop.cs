using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestDrop", menuName = "GameObjects/ChestRewards/ChestDrop")]
public class ChestDrop : ScriptableObject
{
    public Sprite Icon;
    public CollectibleItem drop;
    public int AmountMin;
    public int AmountMax;
    public int chancesProc;
}