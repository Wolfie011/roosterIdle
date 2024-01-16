using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rafinery : BuildingWithTimer
{
    public RafineryUI rafineryUI;
    public Rafinery instance;
    public List<Producible> producibles;
    public bool isStructNew = true;

    [SerializeField] private GameObject windowPrefab;
    public ProductionSave dataPRD = new ProductionSave();
    public Dictionary<CollectibleItem, int> readyProdos = new Dictionary<CollectibleItem, int>();
    private void Awake()
    {
        instance = this;
        Initialize();
    }
    
    public void InitializeOnLoad(ProductionSave rafineryData)
    {
        Debug.Log("Initializing Rafinery ID: "+data.ID);
        isStructNew = false;
        dataPRD = rafineryData;
        //VALIDER
        if (dataPRD.productsQue.Count > 0)
        {
            DateTime validTime = dataPRD.pauzeTime.AddSeconds(dataPRD.secondsLeft);
            Debug.Log($"Finish time: {validTime}, DateTimeNow is: {DateTime.Now}");
            if (validTime > DateTime.Now)
            {
                Debug.Log("Product [0] in que didnt finish yet");
                rafineryUI.InitializeOnLoad(validTime);
            }
            else
            {
                Debug.Log($"Doing for-loop");
                Debug.Log(dataPRD.productsQue.Count);
                for (int i = 0; i < dataPRD.productsQue.Count; i++)
                {
                    Debug.Log($"{i}");
                    if (validTime < DateTime.Now)
                    {
                        Debug.Log($"Product at [{i}] in que finished");
                        StorageManager.current.UpdateItems(getProduct(dataPRD.productsQue[i]).ItemsAquired, false);
                        validTime += getProduct(dataPRD.productsQue[i]).productionTime;
                        dataPRD.productsQue.RemoveAt(0);
                    }
                    else
                    {
                        rafineryUI.InitializeOnLoad(validTime);
                        Debug.Log($"Product at [{i}] in que didnt finish yet");
                        Debug.Log($"Will at {validTime}");
                        return;
                    }
                }
            }
        }
        else
        {
            Debug.Log("No production processes in save file");
        }
    }
    public Product getProduct(string nameOf)
    {
        try
        {
            Product product = Resources.Load<Product>("Storage/" + nameOf);
            return product;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        } 
    }
    private void finishProduct(string nameOf)
    {
        Product product = Resources.Load<Product>("Storage/"+nameOf);
        StorageManager.current.UpdateItems(product.ItemsAquired, true);
        rafineryUI.updateReqAmounts();
    }
    public void Initialize()
    {
        //instantiate UI
        GameObject window = Instantiate(windowPrefab, GameManager.current.canvas.transform);

        //make it invisible
        window.SetActive(false);

        //get the hatcher ui script
        rafineryUI = window.GetComponent<RafineryUI>();

        rafineryUI.parentRafinery = instance;
        rafineryUI.producibles = producibles;

        //initialize the UI
        rafineryUI.Initialize();
    }

    public void newStruct()
    {
        
    }
    protected override void OnClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if(built)
        {
            //initialize hatcher UI so it is updated
            rafineryUI.Initialize();
            //make the UI visible after the click on the building
            rafineryUI.gameObject.SetActive(true);
        }
        else
        {
            base.OnClick();
        }
    }
    private void OnApplicationQuit()
    {
        if (Placed)
        {
            //updating the position of the building for saving
            data.position = transform.position;
        }
        if (gameObject.GetComponent<Timer>() && gameObject.GetComponent<Timer>().secondsLeft != 0)
        {
            dataTM.ID = data.ID;
            dataTM.Name = gameObject.GetComponent<Timer>().Name;
            dataTM.startTime = gameObject.GetComponent<Timer>().startTime;
            dataTM.timeToFinish = gameObject.GetComponent<Timer>().timeToFinish;
            dataTM.finishTime = gameObject.GetComponent<Timer>().finishTime;
            dataTM.pauzeTime = DateTime.Now;
            dataTM.secondsLeft = gameObject.GetComponent<Timer>().secondsLeft;
            GameManager.current.saveData.AddData(dataTM);
        }
        data.isNew = isNew;
        data.area = area;
        data.buildType = buildType;
        data.built = built;
        data.building = building;
        GameManager.current.saveData.AddData(data);

        if (isStructNew)
        {
            newStruct();
        }

        dataPRD.ID = data.ID;

        dataPRD.productsQue.Clear();

        foreach(var prodo in rafineryUI.currentQueue)
        {
            dataPRD.productsQue.Add(prodo.Name);
        }

        if (rafineryUI.gameObject.GetComponent<Timer>()) dataPRD.secondsLeft = rafineryUI.timer.secondsLeft;
        else dataPRD.secondsLeft = 0;

        dataPRD.pauzeTime = DateTime.Now;

        GameManager.current.saveData.AddData(dataPRD);
    }
}
