using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Product", menuName = "GameObjects/StorageItems/Product")]
public class Product : Producible
{
    private new void OnValidate()
    {
        base.OnValidate();

        //ItemsNeeded = new Dictionary<CollectibleItem, int>() { { this, 1 } };
    }
}
