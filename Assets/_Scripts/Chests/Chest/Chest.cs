using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest", menuName = "GameObjects/ChestRewards/Chest")]
public class Chest : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public List<ChestDrop> drops;
}