using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GameManager : MonoBehaviour
{
    //singletone pattern
    public static GameManager current;

    //the main object to be saved into a file
    public SaveData saveData;
    //path for loading scriptable objects (ShopItems)
    [SerializeField] private string shopItemsPath = "Shop";
    [SerializeField] private GameObject popupPrefab;

    //save the canvas
    public GameObject canvas;

    private void Awake()
    {
        //initialize fields
        current = this;

        ShopItemDrag.canvas = canvas.GetComponent<Canvas>();
        UIDrag.canvas = canvas.GetComponent<Canvas>();

        //initialize the save system
        SaveSystem.Initialize();
    }

    private void Start()
    {
        //load the save data
        saveData = SaveSystem.Load();
        //load the game
        LoadGame();
    }

    private void LoadGame()
    {
        LoadItems();
        LoadPlaceableObjects();
        //LoadProduction();
        LoadLevel();
        LoadCurrency();
        LoadRewards();
        LoadCurrentQuests();
    }
    
    private void LoadPlaceableObjects()
    {
        List<TimerData> timersToRemove = new List<TimerData>();

        //go through each saved data
        foreach (var plObjData in saveData.placeableObjectDatas.Values)
        {
            //try-catch block in case something wasn't saved properly
            //to avoid errors
            try
            {
                //get the ShopItem from resources
                ShopItem item = Resources.Load<ShopItem>(shopItemsPath + "/" + plObjData.assetName);
                //instantiate the prefab
                GameObject obj = GridBuildingSystem.current.InitializeWithObject(item.Prefab, plObjData.position);
                //get the placeable object component
                Building plObj = obj.GetComponent<Building>();
                //initialize the component
                plObj.Initialize(item, plObjData);
                plObj.Placed = true;
                plObj.isNew = plObjData.isNew;
                plObj.built = plObjData.built;
                plObj.building = plObjData.building;

                GridBuildingSystem.current.LoadPlacer(plObjData.area);

                foreach (var timerData in saveData.timerDatas.Values)
                {
                    if (timerData.ID == plObjData.ID)
                    {
                        Debug.Log("Timer matches Building");

                        if (timerData.finishTime > DateTime.Now)
                        {
                            Debug.Log("Timer good");
                            plObj.InitializeTimerOnLoad(timerData, false);
                        }
                        else if (timerData.finishTime < DateTime.Now)
                        {
                            Debug.Log("Timer expired");
                            plObj.InitializeTimerOnLoad(timerData, true);
                            //saveData.RemoveData(timerData);
                            timersToRemove.Add(timerData);
                        }
                    }
                }

                if (plObjData.buildType == buildType.Chicken)
                {
                    switch (plObjData.assetName)
                    {
                        case "Chicken Hatcher":
                            Hatcher hatcher = obj.GetComponent<Hatcher>();
                            foreach(var hatcherData in saveData.hatcherSaves.Values)
                            {
                                if(hatcherData.ID == plObjData.ID)
                                {
                                    Debug.Log("HatchData matches Building ID");
                                    //hatcher.isStructNew = false;
                                    hatcher.InitializeOnLoad(hatcherData);
                                }
                            }
                            break;
                        case "Chicken Breeder":
                            Breeder breeder = obj.GetComponent<Breeder>();
                            foreach (var breederData in saveData.breederSaves.Values)
                            {
                                if (breederData.ID == plObjData.ID)
                                {
                                    Debug.Log("BreedData matches Building ID");
                                    //breeder.isStructNew = false;
                                    breeder.InitializeOnLoad(breederData);
                                }
                            }
                            break;
                        case "Chicken Coop":
                            Coop coop = obj.GetComponent<Coop>();
                            foreach (var coopData in saveData.coopSaves.Values)
                            {
                                if (coopData.ID == plObjData.ID)
                                {
                                    Debug.Log("CoopData matches Building ID");
                                    //breeder.isStructNew = false;
                                    coop.InitializeOnLoad(coopData);
                                }
                            }
                            break;
                    }
                }
                if (plObjData.buildType == buildType.Production)
                {
                    switch (plObjData.assetName)
                    {
                        case "Field":
                            Field field = obj.GetComponent<Field>();
                            foreach (var fieldData in saveData.fieldData.Values)
                            {
                                if (fieldData.ID == plObjData.ID)
                                {
                                    Debug.Log("FieldData matches Building ID");
                                    field.InitializeOnLoad(fieldData);
                                }
                            }
                            break;

                        case "Blacksmith":
                            ProductionBuilding productionBuilding = obj.GetComponent<ProductionBuilding>();
                            foreach (var productionData in saveData.productionSave.Values)
                            {
                                if (productionData.ID == plObjData.ID)
                                {
                                    Debug.Log("Production (BLACKSMITH) data matches Building ID");
                                    productionBuilding.InitializeOnLoad(productionData);
                                    if (!saveData.timerDatas.ContainsKey(productionData.ID))
                                    {
                                        productionBuilding.building = false;
                                        productionBuilding.built = true;
                                    }
                                }
                            }
                            break;
                        case "Rafinery":
                            Rafinery rafinery = obj.GetComponent<Rafinery>();
                            foreach (var rafineryData in saveData.productionSave.Values)
                            {
                                if (rafineryData.ID == plObjData.ID)
                                {
                                    //Debug.Log("Production (RAFINERY) data matches Building ID");
                                    rafinery.InitializeOnLoad(rafineryData);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        for(int i = 0; i < timersToRemove.Count; i++)
        {
            saveData.RemoveData(timersToRemove[i]);
        }
        foreach(var blockData in saveData.blockData.Values)
        {
            try
            {
                ShopItem item = Resources.Load<ShopItem>(shopItemsPath + "/" + blockData.assetName);
                Territory territory = Instantiate(item.block, blockData.position, Quaternion.identity);
                territory.bData = blockData;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    private void LoadLevel()
    {
        try
        {
            LevelSystem.Level = saveData.levelData.Level;
            LevelSystem.xpToNext = saveData.levelData.xpToNext;
            LevelSystem.XPNow = saveData.levelData.XPNow;

            LevelSystem.current.UpdateOnLoad();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private void LoadCurrency()
    {
        try
        {
            CurrencySystem.current.CurrencyAmounts[CurrencyType.Coins] = saveData.currencyData.coins;
            CurrencySystem.current.CurrencyAmounts[CurrencyType.Crystals] = saveData.currencyData.crystals;
            CurrencySystem.current.UpdateUI();
            LevelSystem.current.UpdateOnLoad();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private void LoadRewards()
    {
        try
        {
            if(saveData.dailyRewardsData.finishTime > DateTime.Now)
            {
                DailyRewardsManager.current.InitializeOnLoad(saveData.dailyRewardsData, false);
            }
            else
            {
                DailyRewardsManager.current.InitializeOnLoad(saveData.dailyRewardsData, true);
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        try
        {
            if (saveData.luckyDatas.finishTime > DateTime.Now)
            {
                LuckyWheelManager.current.InitializeOnLoad(saveData.luckyDatas, false);
            }
            else
            {
                LuckyWheelManager.current.InitializeOnLoad(saveData.luckyDatas, true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public void LoadItems()
    {
        foreach(var item in saveData.storageDatas)
        {
            try
            {
                int amount = item.amount;
                if(amount < 0) amount = 0;
                CollectibleItem collectibleItem = Resources.Load<CollectibleItem>("Storage" + "/" + item.itemName);
                Dictionary<CollectibleItem, int> toADD = new Dictionary<CollectibleItem, int>
                {
                    { collectibleItem, amount}
                };
                StorageManager.current.UpdateItems(toADD, true);
                //Debug.Log(collectibleItem.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        foreach (var chest in saveData.chestDatas)
        {
            try
            {
                StorageManager.current.chests.Add(chest.ID, StorageManager.current.ChestList[chest.chestIndex]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        foreach (var chicken in saveData.chickenDatas.Values)
        {
            try
            {
                Chicken chickenLoad = Resources.Load<Chicken>("Chickens/" + chicken.assetName);
                Chicken chickenLocal = Instantiate(chickenLoad);

                chickenLocal.ID = chicken.ID;
                chickenLocal.Strength = chicken.Strength;
                chickenLocal.Growth = chicken.Growth;
                chickenLocal.Gain = chicken.Gain;
                chickenLocal.inUse = chicken.inUse;
                chickenLocal.breedTime = chickenLoad.breedTime;

                StorageManager.current.chickens.Add(chicken.ID, chickenLocal);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        foreach (var egg in saveData.eggDatas.Values)
        {
            try
            {
                Egg eggLoad = Resources.Load<Egg>("Eggs/" + egg.assetName);
                Egg eggLocal = Instantiate(eggLoad);

                eggLocal.ID = egg.ID;
                eggLocal.Strength = egg.Strength;
                eggLocal.Growth = egg.Growth;
                eggLocal.Gain = egg.Gain;
                eggLocal.inUse = egg.inUse;
                eggLocal.productionTime = eggLoad.productionTime;

                StorageManager.current.eggs.Add(egg.ID, eggLocal);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    public void LoadCurrentQuests()
    {
        try
        {
            QuestManager.current.InitializeOnLoad(saveData.questSave);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private void OnDisable()
    {
        //save the data before closing
        SaveSystem.Save(saveData);
    }
    public void TriggerPopUp()
    {
        PopUpADs popUpADsPrefab = Instantiate(popupPrefab, canvas.transform).GetComponent<PopUpADs>();
        AdsInitializer.current.InitializePopUp(popUpADsPrefab);
    }
    public void GetXP(int amount)
    {
        XPAddedGameEvent info = new XPAddedGameEvent(amount);
        EventManager.Instance.QueueEvent(info);
    }
    public void GetCoins(int amount)
    {
        CurrencyChangeGameEvent info = new CurrencyChangeGameEvent(amount, CurrencyType.Coins);
        EventManager.Instance.QueueEvent(info);
    }
    public void GetCrystals(int amount)
    {
        CurrencyChangeGameEvent info = new CurrencyChangeGameEvent(amount, CurrencyType.Crystals);
        EventManager.Instance.QueueEvent(info);
    }
    public void GetCollectableItem(int amount, CollectibleItem item)
    {
        Dictionary<CollectibleItem, int> tempDictionary = new Dictionary<CollectibleItem, int>
        {
            { item, amount }
        };
        StorageManager.current.UpdateItems(tempDictionary, true);
    }
    public void GetEgg(Egg egg, List<int> paramS)
    {
        Egg localEgg = Instantiate(egg);

        localEgg.Strength = paramS[0];
        localEgg.Growth = paramS[1];
        localEgg.Gain = paramS[2];

        localEgg.ID = SaveData.GenerateUUID();
        localEgg.productionTime = egg.productionTime;

        StorageManager.current.eggs.Add(localEgg.ID, localEgg);
    }
}
