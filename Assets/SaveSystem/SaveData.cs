using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class SaveData
{
    //id count for generating a new id
    public static int IdCount;

    //all the placeable objects on the map
    public Dictionary<string, PlaceableObjectData> placeableObjectDatas = new Dictionary<string, PlaceableObjectData>();
    public LevelData levelData = new LevelData();
    public CurrencyData currencyData = new CurrencyData();
    public DailyRewardsData dailyRewardsData = new DailyRewardsData();
    public Dictionary<string, TimerData> timerDatas = new Dictionary<string, TimerData>();
    public List<StorageData> storageDatas = new List<StorageData>();
    public List<ChestData> chestDatas = new List<ChestData>();
    public LuckyData luckyDatas = new LuckyData();
    public Dictionary<string, BlockData> blockData = new Dictionary<string, BlockData>();
    public Dictionary<string, QuestSave> questSave = new Dictionary<string, QuestSave>();

    public Dictionary<string, FieldData> fieldData = new Dictionary<string, FieldData>();

    public Dictionary<string, ChickenData> chickenDatas = new Dictionary<string, ChickenData>();
    public Dictionary<string, EggData> eggDatas = new Dictionary<string, EggData>();


    public Dictionary<string, HatcherSave> hatcherSaves = new Dictionary<string, HatcherSave>();
    public Dictionary<string, BreederSave> breederSaves = new Dictionary<string, BreederSave>();
    public Dictionary<string, CoopSave> coopSaves = new Dictionary<string, CoopSave>();

    public Dictionary<string, ProductionSave> productionSave = new Dictionary<string, ProductionSave>();
    public static string GenerateId()
    {
        //increase id
        IdCount++;
        //return it as string
        return IdCount.ToString();
    }
    public static string GenerateUUID()
    {
        Guid guid = Guid.NewGuid();
        string uuid = guid.ToString();
        uuid = uuid.Replace("-", "").ToLower();
        return uuid;
    }

    public void AddData(Data data)
    {
        //check the type of data
        if (data is BlockData bData)
        {
            //if it is already in the dictionary
            if (blockData.ContainsKey(bData.ID))
            {
                //update the information
                blockData[bData.ID] = bData;
            }
            else
            {
                //add a new object to save
                blockData.Add(bData.ID, bData);
            }
        }
        if (data is PlaceableObjectData plObjData)
        {
            //if it is already in the dictionary
            if (placeableObjectDatas.ContainsKey(plObjData.ID))
            {
                //update the information
                placeableObjectDatas[plObjData.ID] = plObjData;
            }
            else
            {
                //add a new object to save
                placeableObjectDatas.Add(plObjData.ID, plObjData);
            }
        }
        if (data is HatcherSave hatcherSaveData)
        {
            //if it is already in the dictionary
            if (hatcherSaves.ContainsKey(hatcherSaveData.ID))
            {
                //update the information
                hatcherSaves[hatcherSaveData.ID] = hatcherSaveData;
            }
            else
            {
                //add a new object to save
                hatcherSaves.Add(hatcherSaveData.ID, hatcherSaveData);
            }
        }
        if (data is BreederSave breederSaveData)
        {
            //if it is already in the dictionary
            if (breederSaves.ContainsKey(breederSaveData.ID))
            {
                //update the information
                breederSaves[breederSaveData.ID] = breederSaveData;
            }
            else
            {
                //add a new object to save
                breederSaves.Add(breederSaveData.ID, breederSaveData);
            }
        }
        if (data is CoopSave coopSaveData)
        {
            if (coopSaves.ContainsKey(coopSaveData.ID))
            {
                //update the information
                coopSaves[coopSaveData.ID] = coopSaveData;
            }
            else
            {
                //add a new object to save
                coopSaves.Add(coopSaveData.ID, coopSaveData);
            }
        }
        if (data is LevelData lvlData)
        {
            if (levelData.Level != LevelSystem.Level || levelData.XPNow != LevelSystem.XPNow)
            {
                levelData.Level = lvlData.Level;
                levelData.XPNow = lvlData.XPNow;
                levelData.xpToNext = lvlData.xpToNext;
            }
        }
        if (data is CurrencyData currData)
        {
            currencyData = currData;
        }
        if (data is DailyRewardsData dailyData)
        {
            dailyRewardsData = dailyData;
        }
        if (data is TimerData timerData)
        {
            //if it is already in the dictionary
            if (timerDatas.ContainsKey(timerData.ID))
            {
                //update the information
                timerDatas[timerData.ID] = timerData;
            }
            else
            {
                //add a new object to save
                timerDatas.Add(timerData.ID, timerData);
            }
        }
        if (data is StorageData storageData)
        {
            int exists = storageDatas.FindIndex(item => item.itemName == storageData.itemName);
            if (exists != -1)
            {
                storageDatas[exists].amount = storageData.amount;
                Debug.Log("Updated amound: " + storageDatas[exists].amount);
            }
            else
            {
                storageDatas.Add(storageData);
            }
            
        }
        if (data is ChestData chestData)
        {
            int exists = chestDatas.FindIndex(item => item.ID == chestData.ID);
            if (exists != -1)
            {
                //Debug.Log("Already is in LIST");
            }
            else
            {
                chestDatas.Add(chestData);
            }
        }
        if (data is LuckyData luckyData)
        {
            luckyDatas = luckyData;
        }
        if (data is EggData eggData)
        {
            if (eggDatas.ContainsKey(eggData.ID))
            {
                eggDatas[eggData.ID] = eggData;
            }
            else
            {
                eggDatas.Add(eggData.ID, eggData);
            }
        }
        if (data is ChickenData chickenData)
        {
            if (chickenDatas.ContainsKey(chickenData.ID))
            {
                chickenDatas[chickenData.ID] = chickenData;
            }
            else
            {
                chickenDatas.Add(chickenData.ID, chickenData);
            }
        }
        if (data is QuestSave questSaveData)
        {
            if (questSave.ContainsKey(questSaveData.ID))
            {
                questSave[questSaveData.questsID] = questSaveData;
            }
            else
            {
                questSave.Add(questSaveData.questsID, questSaveData);
            }
        }
        if (data is FieldData fData)
        {
            if (fieldData.ContainsKey(fData.ID))
            {
                fieldData[fData.ID] = fData;
            }
            else
            {
                fieldData.Add(fData.ID, fData);
            }
        }
        if (data is ProductionSave productionSaveData)
        {
            if (productionSave.ContainsKey(productionSaveData.ID))
            {
                productionSave[productionSaveData.ID] = productionSaveData;
            }
            else
            {
                productionSave.Add(productionSaveData.ID, productionSaveData);
            }
        }
    }
    public void RemoveData(Data data)
    {
        //check the type of data
        if (data is PlaceableObjectData plObjData)
        {
            //check if it is in the dictionary
            if (placeableObjectDatas.ContainsKey(plObjData.ID))
            {
                //remove it from the dictionary
                placeableObjectDatas.Remove(plObjData.ID);
            }
        }
        if (data is TimerData timerData)
        {
            //check if it is in the dictionary
            if (timerDatas.ContainsKey(timerData.ID))
            {
                Debug.Log("Removed timer ID: " + timerData.ID);
                //remove it from the dictionary
                timerDatas.Remove(timerData.ID);
            }
        }
        if (data is ChickenData chickenData)
        {
            if (chickenDatas.ContainsKey(chickenData.ID))
            {
                Debug.Log("Removed CHICKEN ID: " + chickenData.ID);
                chickenDatas.Remove(chickenData.ID);
            }
        }
        if (data is EggData eggData)
        {
            if (eggDatas.ContainsKey(eggData.ID))
            {
                Debug.Log("Removed EGG ID: " + eggData.ID);
                eggDatas.Remove(eggData.ID);
            }
        }
    }
    public void RemoveSpecial(string key, int index)
    {
        switch (index)
        {
            case 0:
                if(eggDatas.ContainsKey(key))
                {
                    eggDatas.Remove(key);
                }
                break;
            case 1:
                if (chickenDatas.ContainsKey(key))
                {
                    chickenDatas.Remove(key);
                }
                break;
            case 2:
                var chestToRemove = chestDatas.FirstOrDefault(r => r.ID == key);
                if (chestToRemove == null) break;
                else
                {
                    chestDatas.Remove(chestToRemove);
                }
                break;
            case 3:
                blockData.Remove(key);
                break;
        }
    }
    
    //this method called when deserializing the object
    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        //in case the data dictionary is null, create it
        placeableObjectDatas ??= new Dictionary<string, PlaceableObjectData>();

        levelData ??= new LevelData();
        currencyData ??= new CurrencyData();
        dailyRewardsData ??= new DailyRewardsData();
        timerDatas ??= new Dictionary<string, TimerData>();
        storageDatas ??= new List<StorageData>();
        chestDatas ??= new List<ChestData>();
        luckyDatas ??= new LuckyData();

        hatcherSaves ??= new Dictionary<string, HatcherSave>();
        breederSaves ??= new Dictionary<string, BreederSave>();
        coopSaves ??= new Dictionary<string, CoopSave>();

        chickenDatas ??= new Dictionary<string, ChickenData>();
        eggDatas ??= new Dictionary<string, EggData>();

        questSave ??= new Dictionary<string, QuestSave>();

        fieldData ??= new Dictionary<string, FieldData>();
        productionSave ??= new Dictionary<string, ProductionSave>();

        blockData ??= new Dictionary<string, BlockData>();
    }
}
