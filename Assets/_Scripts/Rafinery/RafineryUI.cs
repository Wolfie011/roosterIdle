using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RafineryUI : MonoBehaviour
{
    #region UI
    public Rafinery parentRafinery;
    public List<Producible> producibles;
    [SerializeField] private GameObject recitePrefab;
    [SerializeField] private GameObject productPrefab;

    [SerializeField] private GameObject producibleView;
    [SerializeField] private Transform reciteContent;
    
    [SerializeField] private GameObject productionView;
    [SerializeField] private Transform productionContent;
    #endregion

    //state of the production
    public State currentState { get; set; }
    //current production queue
    [SerializeField] public Queue<Producible> currentQueue = new Queue<Producible>();
    //current item in progress
    private Producible currentProd;

    public Timer timer;

    private ProductionSave productionSaveData = new ProductionSave();
    public void Initialize()
    {
        InitializeRecites();
    }
    public void InitializeRecites()
    {
        int childCount = reciteContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(reciteContent.GetChild(i).gameObject);
        }
        foreach (var recite in producibles)
        {
            GameObject reciteInst = Instantiate(recitePrefab, reciteContent);
            reciteInst.GetComponent<RecipePrefab>().producible = recite;
            reciteInst.GetComponent<RecipePrefab>().rafineryUI = this;
        }
    }
    public void createNewProcess(Producible product)
    {
        productionItem newProcess = Instantiate(productPrefab, productionContent).GetComponent<productionItem>();
        newProcess.producible = product;
        newProcess.rafineryUI = this;

        currentQueue.Enqueue(product);

        checkQueue();
    }
    public void StartNextProcess()
    {
        currentProd = currentQueue.Peek();
        productionContent.GetChild(0).gameObject.GetComponent<productionItem>().countdown = true;

        timer.Initialize(SaveData.GenerateUUID(), DateTime.Now, currentProd.productionTime);
        timer.TimerFinishedEvent.AddListener(delegate
        {
            Debug.Log("Ended classic");
            currentState = State.Ready;
            currentQueue.Dequeue();
            StorageManager.current.UpdateItems(currentProd.ItemsAquired, true);
            updateReqAmounts();
            Destroy(productionContent.GetChild(0).gameObject);

            if (currentQueue.Count > 0)
            {
                productionContent.GetChild(1).gameObject.GetComponent<productionItem>().countdown = true;

                StartNextProcess();
            }
            else
            {
                currentState = State.Empty;
                Destroy(timer);
                timer = null;
            }
        });
        timer.StartTimer();
    }
    public bool checkQueue()
    {
        if (currentState == State.Empty)
        {
            currentState = State.InProgress;
            //add the timer
            timer = gameObject.AddComponent<Timer>();
            //handle the queue
            StartNextProcess();

            return true;
        }
        else return false;
    }
    public void InitializeOnLoad(DateTime validtime)
    {
        foreach(var item in parentRafinery.dataPRD.productsQue)
        {
            //Debug.Log(item);
            productionItem newProcess = Instantiate(productPrefab, productionContent).GetComponent<productionItem>();
            newProcess.producible = parentRafinery.getProduct(item);
            newProcess.rafineryUI = this;

            currentQueue.Enqueue(parentRafinery.getProduct(item));
        }

        currentState = State.InProgress;
        //add the timer
        timer = gameObject.AddComponent<Timer>();

        currentProd = currentQueue.Peek();
        productionContent.GetChild(0).gameObject.GetComponent<productionItem>().countdown = true;
        
        timer.Initialize(SaveData.GenerateUUID(), DateTime.Now, validtime.TimeOfDay);
        timer.TimerFinishedEvent.AddListener(delegate
        {
            currentState = State.Ready;
            currentQueue.Dequeue();
            StorageManager.current.UpdateItems(currentProd.ItemsAquired, true);
            Debug.Log("Added via loaded data timer");
            updateReqAmounts();
            Destroy(productionContent.GetChild(0).gameObject);
            Debug.Log("Koniec dodanego");
            if (currentQueue.Count > 0)
            {
                productionContent.GetChild(1).gameObject.GetComponent<productionItem>().countdown = true;

                StartNextProcess();
            }
            else
            {
                currentState = State.Empty;
                Destroy(timer);
                timer = null;
            }
        });
        timer.StartTimer();
        Debug.Log(validtime.TimeOfDay.TotalSeconds);
    }
    public void addRest(DateTime validTime, int index)
    {
        for(int i = index; i < parentRafinery.dataPRD.productsQue.Count; i++)
        {
            productionItem newProcess = Instantiate(productPrefab, productionContent).GetComponent<productionItem>();
            newProcess.producible = parentRafinery.getProduct(parentRafinery.dataPRD.productsQue[i]);
            newProcess.rafineryUI = this;

            currentQueue.Enqueue(parentRafinery.getProduct(parentRafinery.dataPRD.productsQue[i]));
        }

        currentState = State.InProgress;
        //add the timer
        timer = gameObject.AddComponent<Timer>();

        timer.Initialize(SaveData.GenerateUUID(), DateTime.Now, validTime.TimeOfDay);
        timer.TimerFinishedEvent.AddListener(delegate
        {
            currentState = State.Ready;
            currentQueue.Dequeue();
            StorageManager.current.UpdateItems(currentProd.ItemsAquired, true);
            Debug.Log("Added via loaded data timer");
            updateReqAmounts();
            Destroy(productionContent.GetChild(0).gameObject);

            if (currentQueue.Count > 0)
            {
                productionContent.GetChild(1).gameObject.GetComponent<productionItem>().countdown = true;

                StartNextProcess();
            }
            else
            {
                currentState = State.Empty;
                Destroy(timer);
                timer = null;
            }
        });
        timer.StartTimer();
    }
    public void updateReqAmounts()
    {
        int childCount = reciteContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            reciteContent.GetChild(i).GetComponent<RecipePrefab>().updateAmounts();
        }
    }
}

