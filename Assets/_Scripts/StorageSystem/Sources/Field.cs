using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Field : Building, ISource
{
    //all crops that can be planted
    private static Dictionary<Crop, int> allCrops;
    //the default amount of crop produced (can be changed for events, for example)

    //private static int amount = 2;
    
    //implementation of the state property
    public State currentState { get; set; }
    //current crop in production
    private Crop currentCrop;
    //timer for the crop
    private Timer timer;
    
    //sprite renderer
    private SpriteRenderer sr;
    //empty field sprite (set after the crop is collected)
    private Sprite emptyFieldSprite;

    public FieldData dataFLD = new FieldData();
    public bool isStructNew = true;

    private void Awake()
    {
        //get the somponent
        sr = GetComponent<SpriteRenderer>();
        //save the sprite
        emptyFieldSprite = sr.sprite;
    }
    public void InitializeOnLoad(FieldData fdata)
    {
        isStructNew = false;
        dataFLD = fdata;
        if(dataFLD.fieldState == State.Empty) { }
        if(dataFLD.fieldState == State.InProgress)
        {
            if (dataFLD.timerData.finishTime > DateTime.Now)
            {
                currentCrop = Resources.Load<Crop>($"Storage/{dataFLD.currCropName}");

                currentState = State.InProgress;
                sr.sprite = currentCrop.growingCrop;

                double secondsL = dataFLD.timerData.secondsLeft - (DateTime.Now - dataFLD.timerData.pauzeTime).TotalSeconds;
                timer = gameObject.AddComponent<Timer>();
                //initialize the timer
                timer.Initialize(currentCrop.name, DateTime.Now, TimeSpan.FromSeconds(secondsL));
                //add a listener to the timer finished event
                timer.TimerFinishedEvent.AddListener(delegate
                {
                    //change the state
                    currentState = State.Ready;
                    //change the sprite
                    sr.sprite = currentCrop.readyCrop;

                    //destroy the timer
                    Destroy(timer);
                    //nullify the timer
                    timer = null;
                });
                timer.StartTimer();
            }
            else readyState();
        }
        if(dataFLD.fieldState == State.Ready)
        {
            readyState();
        }

        void readyState()
        {
            currentCrop = Resources.Load<Crop>($"Storage/{dataFLD.currCropName}");

            currentState = State.Ready;
            //change the sprite
            sr.sprite = currentCrop.readyCrop;
        }
    }
    public static void Initialize(Dictionary<Crop, int> crops)
    {
        //initialize all the crops
        allCrops = crops;
    }

    protected override void OnClick()
    {
        //check the state
        if (GridBuildingSystem.current.temp == null)
        {
            switch (currentState)
            {
                case State.Empty:
                    //the field is empty -> display the crops available to plant
                    ItemsTooltip.ShowTooltip_Static(gameObject, allCrops);
                    break;
                case State.InProgress:
                    //a crop is growing on the field -> display the timer
                    TimerTooltip.ShowTimer_Static(gameObject);
                    break;
                case State.Ready:
                    //the field is ready -> display the tooltip to collect
                    CollectorTooltip.ShowTooltip_Static(gameObject);
                    break;
            }
        }   
    }

    //implementation of the produce method
    public void Produce(Dictionary<CollectibleItem, int> itemsNeeded, CollectibleItem itemToProduce)
    {
        //if the field is not empty nothing to do
        if (currentState != State.Empty)
        {
            return;
        }

        //check if the product is a crop
        if (itemToProduce is Crop crop)
        {
            //assign the crop
            currentCrop = crop;
            //Debug.Log(currentCrop);
        }
        else
        {
            return;
        }

        //check if the player has enough items
        foreach (var itemPair in itemsNeeded)
        {
            //if not enough -> return
            if (!StorageManager.current.IsEnoughOf(itemPair.Key, itemPair.Value))
            {
                Debug.Log("Not enough items");
                return;
            }
        }

        //create a dictionary for the result
        //Dictionary<CollectibleItem, int> result = new Dictionary<CollectibleItem, int>();
        //get the item from the produced queue and add it to the result
        //result.Add(currentCrop, 1);
        //add the items to storage manager
        StorageManager.current.UpdateItems(currentCrop.ItemsNeeded, false);

        //change the state
        currentState = State.InProgress;

        //change the sprite
        sr.sprite = currentCrop.growingCrop;

        //add a timer
        timer = gameObject.AddComponent<Timer>();
        //initialize the timer
        timer.Initialize(currentCrop.name, DateTime.Now, currentCrop.productionTime);
        //add a listener to the timer finished event
        timer.TimerFinishedEvent.AddListener(delegate
        {
            //change the state
            currentState = State.Ready;
            //change the sprite
            sr.sprite = currentCrop.readyCrop;
            
            //destroy the timer
            Destroy(timer);
            //nullify the timer
            timer = null;
        });
        timer.StartTimer();
    }

    public void Collect()
    {
        if (currentCrop != null && currentState == State.Ready)
        {
            EventManager.Instance.QueueEvent(new HarvestingGameEvent(currentCrop.Name));
            //create a dictionary for the result
            Dictionary<CollectibleItem, int> result = new Dictionary<CollectibleItem, int>();
            //get the item from the produced queue and add it to the result
            result.Add(currentCrop, 9);
            //add the items to storage manager
            StorageManager.current.UpdateItems(result, true);
        }


        //change the stage
        currentState = State.Empty;
        //change the sprite to empty
        sr.sprite = emptyFieldSprite;
        //remove the current crop
        currentCrop = null;
    }
    private void newStruct()
    {
        dataFLD.currCropName = "";
        dataFLD.fieldState = State.Empty;
        TimerData TM = new TimerData();
        TM.startTime = new DateTime(2000, 10, 10, 10, 20, 30); ;
        TM.secondsLeft = 0;
        TM.finishTime = new DateTime(2000, 10, 10, 10, 20, 30); ;
        TM.pauzeTime = DateTime.Now;
        dataFLD.timerData = TM;
    }
    private void OnApplicationQuit()
    {
        if (Placed)
        {
            //updating the position of the building for saving
            data.position = transform.position;
            data.area = area;
            data.buildType = buildType;
            //Adding the data to the main save object
            GameManager.current.saveData.AddData(data);
        }

        if (isStructNew) newStruct();

        if (currentCrop == null) dataFLD.currCropName = "";
        else dataFLD.currCropName = currentCrop.Name;
        dataFLD.fieldState = currentState;
        if (gameObject.GetComponent<Timer>())
        {
            dataFLD.timerData.startTime = gameObject.GetComponent<Timer>().startTime;
            dataFLD.timerData.secondsLeft = gameObject.GetComponent<Timer>().secondsLeft;
            dataFLD.timerData.finishTime = gameObject.GetComponent<Timer>().finishTime;
        }
        else
        {
            dataFLD.timerData.startTime = new DateTime(2000, 10, 10, 10, 20, 30);
            dataFLD.timerData.secondsLeft = 0;
            dataFLD.timerData.finishTime = new DateTime(2000, 10, 10, 10, 20, 30);
        }
        dataFLD.timerData.pauzeTime = DateTime.Now;
        dataFLD.ID = data.ID;

        GameManager.current.saveData.AddData(dataFLD);
    }
}
