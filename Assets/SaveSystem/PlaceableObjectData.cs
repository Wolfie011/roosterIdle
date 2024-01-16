using System;
using UnityEngine;

[Serializable]
public class PlaceableObjectData : Data
{
    //ShopItem name to load it from the resources
    public string assetName;
    //position of the object on the map
    public Vector3 position;
    public buildType buildType;
    public BoundsInt area;
    public bool isNew;
    public bool building;
    public bool built;
}
