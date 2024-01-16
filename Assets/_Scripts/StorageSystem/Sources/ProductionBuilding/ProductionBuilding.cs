using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionBuilding : BuildingWithTimer, ISource
{
    //all products this building can produce
    [SerializeField] private List<Producible> allProducts;
    //maximum number of slots - can be increased by the player
    private int maxSlots = 3;
    //number of goods produced - can be increased during special events
    private int prodAmount = 1;
    
    //state of the production
    public State currentState { get; set; }
    //current production queue
    private Queue<Producible> currentQueue = new Queue<Producible>();
    //already produced goods
    private Queue<Producible> produced = new Queue<Producible>();
    //current item in progress
    private Producible currentProd;
    //timer for the current product
    private Timer timer;

    private ProductionSave productionSaveData = new ProductionSave();
    protected override void OnClick()
    {
        //if the building is built the behavior is different
        if (built)
        {
            if (GridBuildingSystem.current.temp == null)
            {
                //check the state to do the appropriate action
                switch (currentState)
                {
                    case State.Empty:
                        //display the possible items to produce
                        ItemsTooltip.ShowTooltip_Static(gameObject, allProducts);
                        break;
                    case State.InProgress:
                        //display the possible items to produce AND the item queue
                        ItemsTooltip.ShowTooltip_Static(gameObject, allProducts, currentQueue.ToList());
                        break;
                    case State.Ready:
                        //collect the produce on click
                        Collect();
                        break;
                }
            }
        }
        else
        {
            //call the regular building behavior
            base.OnClick();
        }
    }

    public bool isStructNew = true;
    /*
     * Method to handle the item queue
     */
    public void InitializeOnLoad(ProductionSave productionData)
    {
        isStructNew = false;
        productionSaveData = productionData;
    }
    private void StartNextProduction()
    {
        //get the next item
        //but don't take it out yet
        //because we need it for displaying in the slot
        currentProd = currentQueue.Peek();
        
        //initialize the timer with item name, and the time it takes to make it
        timer.Initialize(currentProd.name, DateTime.Now, currentProd.productionTime);
        //when the timer is finished
        timer.TimerFinishedEvent.AddListener(delegate
        {
            //change the state to ready
            currentState = State.Ready;
            //get the item out of the current queue because it is ready
            currentQueue.Dequeue();
            //add it to the produced queue
            produced.Enqueue(currentProd);

            //check if we still have items to produce
            if (currentQueue.Count > 0)
            {
                //start the next cycle
                StartNextProduction();
            }
            else
            {
                //remove the timer
                Destroy(timer);
                timer = null;
            }
            
            //todo display the UI for the item ready
        });
        
        //start the timer
        timer.StartTimer();
    }
    
    public void Produce(Dictionary<CollectibleItem, int> itemsNeeded, CollectibleItem itemToProduce)
    {
        //if the item queue is already full
        if (currentQueue.Count >= maxSlots)
        {
            return;
        }
        
        //check if the item is valid -> it is producible
        if (itemToProduce is Producible prod)
        {
            //check if the player has enough of each item
            foreach (var itemPair in itemsNeeded)
            {
                if (!StorageManager.current.IsEnoughOf(itemPair.Key, itemPair.Value))
                {
                    Debug.Log("Not enough items");
                    return;
                }
            }
            
            //get the items
            //pass false to indicate that we're taking the items
            StorageManager.current.UpdateItems(itemsNeeded, false);
            
            //add the item to the current queue
            currentQueue.Enqueue(prod);

            //if the production is not started
            if (currentState == State.Empty)
            {
                //set the state to in progress
                currentState = State.InProgress;
                //add the timer
                timer = gameObject.AddComponent<Timer>();
                //handle the queue
                StartNextProduction();
            }
            
            //show the UI so it is updated
            ItemsTooltip.ShowTooltip_Static(gameObject, allProducts, currentQueue.ToList());
        }
    }

    public void Collect()
    {
        //create a dictionary for the result
        Dictionary<CollectibleItem, int> result = new Dictionary<CollectibleItem, int>();
        //get the item from the produced queue and add it to the result
        result.Add(produced.Dequeue(), prodAmount);
        //add the items to storage manager
        StorageManager.current.UpdateItems(result, true);

        //check if this was the last item in the produced queue
        if (produced.Count == 0)
        {
            //check if there are still items yet to be produced
            if (currentQueue.Count == 0)
            {
                //no items -> the production building is empty
                currentState = State.Empty;
            }
            else
            {
                //items present -> production lifecycle continues
                currentState = State.InProgress;
            }
        }
        
        Debug.Log("Collected produce");
    }
    private void newStruct()
    {
        for(int i = 0; i <= 3; i++)
        {
            productionSaveData.productsQue.Add(null);
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

        productionSaveData.ID = data.ID;
        if(productionSaveData.productsQue.Count != 0) { productionSaveData.productsQue.Clear(); }
        foreach (var prodo in currentQueue) productionSaveData.productsQue.Add(prodo.Name);
        if (gameObject.GetComponent<Timer>()) productionSaveData.secondsLeft = timer.secondsLeft;
        productionSaveData.pauzeTime = DateTime.Now;
        GameManager.current.saveData.AddData(productionSaveData);
    }
}
