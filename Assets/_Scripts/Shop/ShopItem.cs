using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "GameObjects/Shop Item", order = 0)]
public class ShopItem : ScriptableObject
{
    public bool inStore = true;
    //shop item properties
    public string Name = "Default";
    public string Description = "Description";
    public int Level;
    public int Price;
    public CurrencyType Currency;
    public ObjectType Type;
    public Sprite Icon;
    public GameObject Prefab;
    public Building building;
    //CROP
    //EGG
    public Territory block;
}

//types of objects - also tabs
public enum ObjectType
{
    AnimalHomes,
    Animals,
    ProductionBuildings,
    TreesBushes,
    Decorations
}
