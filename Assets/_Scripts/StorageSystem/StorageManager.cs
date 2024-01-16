using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public List<Chest> ChestList;
    //singletone pattern
    public static StorageManager current;

    //prefabs for storage buildings
    [SerializeField] private GameObject barnPrefab;
    [SerializeField] private GameObject magazinePrefab;
    [SerializeField] private GameObject siloPrefab;

    //path to load collectible items from resources
    private string itemsPath = "Storage";
    //dictionaries to store different types of items
    public Dictionary<Crop, int> crops;
    //private Dictionary<Feed, int> feeds;
    //private Dictionary<Fruit, int> fruits;
    public Dictionary<Product, int> products;
    public Dictionary<Tool, int> tools;

    public Dictionary<string, Chest> chests;
    public Dictionary<string, Chicken> chickens;
    public Dictionary<string, Egg> eggs;

    //dictionary for barn items
    private Dictionary<CollectibleItem, int> barnItems;
    //dictionary for barn items
    private Dictionary<CollectibleItem, int> siloItems;
    //dictionary for magazine items
    private Dictionary<CollectibleItem, int> magazineItems;

    //storage buildings
    private StorageBuilding Barn;
    private StorageBuilding Silo;
    private StorageBuilding Magazine;

    private void Awake()
    {
        //initialize the singletone
        current = this;
        //load all items from resources
        Dictionary<CollectibleItem, int> itemsAmounts = LoadItems();
        //sort all items into different dictionaries
        Sort(itemsAmounts);

        //initialize the field with all the crops
        Field.Initialize(crops);

        LOADBUILDS();
    }

    /*
     * Load collectible items from resources
     */
    private Dictionary<CollectibleItem, int> LoadItems()
    {
        //create a dictionary for all items
        Dictionary<CollectibleItem, int> itemAmounts = new Dictionary<CollectibleItem, int>();
        //load collectible items from resources
        CollectibleItem[] allItems = Resources.LoadAll<CollectibleItem>(itemsPath);

        for (int i = 0; i < allItems.Length; i++)
        {
            //check if the level is less or equal to the current
            if (allItems[i].Level >= LevelSystem.Level)
            {
                //todo remove 2 in a real game
                itemAmounts.Add(allItems[i], 0);
            }
        }

        //return dictionary with items
        return itemAmounts;
    }
    public void ADDITEM(CollectibleItem item)
    {
        Dictionary<CollectibleItem, int> itemS = new Dictionary<CollectibleItem, int>
        {
            { item, 10 }
        };
        UpdateItems(itemS, true);
    }
    public void ADDEGG(Egg egg)
    {
        Egg localEgg = Instantiate(egg);
        localEgg.Strength = 1;
        localEgg.Growth = 1;
        localEgg.Gain = 1;
        localEgg.ID = GenerateUUID();
        localEgg.productionTime = egg.productionTime;
        eggs.Add(localEgg.ID, localEgg);
    }
    public void addchest(int index)
    {
        chests.Add(GenerateUUID(), ChestList[index]);
    }
    /*
     * Sort items into different categories
     */
    private void Sort(Dictionary<CollectibleItem, int> itemsAmounts)
    {
        //initialize dictionaries
        chickens = new Dictionary<string, Chicken>();
        crops = new Dictionary<Crop, int>();
        //feeds = new Dictionary<Feed, int>();
        //fruits = new Dictionary<Fruit, int>();
        products = new Dictionary<Product, int>();
        tools = new Dictionary<Tool, int>();
        eggs = new Dictionary<string, Egg>();

        chests = new Dictionary<string, Chest>();

        siloItems = new Dictionary<CollectibleItem, int>();
        barnItems = new Dictionary<CollectibleItem, int>();
        magazineItems = new Dictionary<CollectibleItem, int>();


        //go through each item and determine the type
        foreach (var itemPair in itemsAmounts)
        {
            if (itemPair.Key is Chicken chicken)
            {
                //add item to the appropriate dictionaries
                //chickens.Add(chicken, itemPair.Value);
                barnItems.Add(chicken, itemPair.Value);
            }
            else if (itemPair.Key is Crop crop)
            {
                crops.Add(crop, itemPair.Value);
                siloItems.Add(crop, itemPair.Value);
            }
            else if (itemPair.Key is Product product)
            {
                products.Add(product, itemPair.Value);
                magazineItems.Add(product, itemPair.Value);
            }
            else if (itemPair.Key is Tool tool)
            {
                tools.Add(tool, itemPair.Value);
                magazineItems.Add(tool, itemPair.Value);
            }
            else if (itemPair.Key is Egg egg)
            {
                //eggs.Add(egg, itemPair.Value);
                barnItems.Add(egg, itemPair.Value);
            }
        }
    }

    //put Barn and Silo on the map
    private void LOADBUILDS()
    {
        //instantiate the silo object   
        GameObject siloObject = GridBuildingSystem.current.InitializeWithObject(siloPrefab, new Vector3(3.3f, -0.825f));
        //get the storage building component and save it
        Silo = siloObject.GetComponent<StorageBuilding>();
        //place the building onto the map
        Silo.Place();
        //initialize with items and a name
        Silo.Initialize(siloItems, "Silo");



        //instantiate the barn object
        GameObject barnObject = GridBuildingSystem.current.InitializeWithObject(barnPrefab, new Vector3(2.2f, -0.225f));
        //get the storage building component and save it
        Barn = barnObject.GetComponent<StorageBuilding>();
        //place the building onto the map
        Barn.Place();
        //initialize with items and a name
        Barn.Initialize(barnItems, "Barn");



        //instantiate the magazin object
        GameObject magazineObject = GridBuildingSystem.current.InitializeWithObject(magazinePrefab, new Vector3(0f, 0.825f));
        //get the storage building component and save it
        Magazine = magazineObject.GetComponent<StorageBuilding>();
        //place the building onto the map
        Magazine.Place();
        //initialize with items and a name
        Magazine.Initialize(magazineItems, "Magazine");
    }

    /*
     * Get amount of an item that the player has
     */
    public int GetAmount(CollectibleItem item)
    {
        //initialize the amount with default value
        int amount = 0;
        //determine the type of an object requested
        if (item is Chicken chicken)
        {
            //try get the amount
            //chickens.TryGetValue(chicken, out amount);
        }
        else if (item is Crop crop)
        {
            crops.TryGetValue(crop, out amount);
        }
        else if (item is Product product)
        {
            products.TryGetValue(product, out amount);
        }
        else if (item is Tool tool)
        {
            tools.TryGetValue(tool, out amount);
        }
        else if (item is Egg egg)
        {
            //eggs.TryGetValue(egg, out amount);
        }

        //return the amount
        return amount;
    }

    /*
     * Check if the player has enough of an item
     * @returns true if the amount the player has is more or equal to the amount required
     */
    public bool IsEnoughOf(CollectibleItem item, int amount)
    {
        return GetAmount(item) >= amount;
    }
    private static string GenerateUUID()
    {
        Guid guid = Guid.NewGuid();
        string uuid = guid.ToString();
        uuid = uuid.Replace("-", "").ToLower();
        return uuid;
    }
    public void UpdateItems(Dictionary<CollectibleItem, int> items, bool add)
    {
        //go through each item in the dictionary
        foreach (var itemPair in items)
        {
            //get the item
            var item = itemPair.Key;
            //get its amount
            var amount = itemPair.Value;

            //if we're not adding
            if (!add)
            {
                //make the amount negative
                amount = -amount;
            }

            //determine the type of the item
            if (item is Chicken chicken)
            {
                //add thee item amount to the appropriate dictionaries
                //chickens[chicken] += amount;
                barnItems[item] += amount;
            }
            else if (item is Crop crop)
            {
                crops[crop] += amount;
                siloItems[item] += amount;
            }
            else if (item is Product product)
            {
                products[product] += amount;
                magazineItems[item] += amount;
            }
            else if (item is Tool tool)
            {
                tools[tool] += amount;
                magazineItems[item] += amount;
            }
            else if (item is Egg egg)
            {
                //eggs[egg] += amount;
                barnItems[item] += amount;
            }
        }
    }
    private void OnApplicationQuit()
    {
        foreach (var item in LoadItems())
        {
            //Debug.Log(item.Key + ": " + item.Value);
            StorageData dataSTR = new StorageData
            {
                itemName = item.Key.Name,
                amount = GetAmount(item.Key)
            };
            //Debug.Log(dataSTR);
            GameManager.current.saveData.AddData(dataSTR);
        }
        if (chests.Count > 0)
        {
            foreach (var chest in chests)
            {
                ChestData dataCHT = new ChestData();
                dataCHT.ID = chest.Key;
                if (chest.Value.Name == "Gold Chest") dataCHT.chestIndex = 0;
                else dataCHT.chestIndex = 1;
                GameManager.current.saveData.AddData(dataCHT);
            }
        }
        if (eggs.Count > 0)
        {
            foreach (var egg in eggs)
            {
                EggData dataEGG = new EggData();

                dataEGG.ID = egg.Value.ID;
                dataEGG.assetName = egg.Value.Name;
                dataEGG.Strength = egg.Value.Strength;
                dataEGG.Growth = egg.Value.Growth;
                dataEGG.Gain = egg.Value.Gain;
                dataEGG.inUse = egg.Value.inUse;

                GameManager.current.saveData.AddData(dataEGG);
            }
        }
        if (chickens.Count > 0)
        {
            foreach (var chicken in chickens)
            {
                ChickenData dataCHK = new ChickenData();

                dataCHK.ID = chicken.Value.ID;
                dataCHK.assetName = chicken.Value.Name;
                dataCHK.Strength = chicken.Value.Strength;
                dataCHK.Growth = chicken.Value.Growth;
                dataCHK.Gain = chicken.Value.Gain;
                dataCHK.inUse = chicken.Value.inUse;

                GameManager.current.saveData.AddData(dataCHK);
            }
        }
    }
}