using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slots : MonoBehaviour
{
    //the object that called the tooltip
    public Building caller { get; private set; }
    //timer for the current product in progress
    private Timer timer;

    //text for the timer
    [SerializeField] private TextMeshProUGUI timeLeftText;
    //slots objects
    [SerializeField] private List<GameObject> slots;

    //if the UI is visible 
    private bool countdown;

    private void Awake()
    {
        //disable the slots at first
        transform.gameObject.SetActive(false);
    }

    public void ShowSlots<T>(GameObject callerObj, List<T> itemsQueue)
    where T : Producible
    {
        //get the caller
        caller = callerObj.GetComponent<Building>();

        //if the queue exists
        if (itemsQueue != null)
        {
            //clear the slots before populating
            ClearSlots();
            //populate the slots
            InitializeSlots(itemsQueue);

            //get the timer
            timer = callerObj.GetComponent<Timer>();

            //ui is visible 
            countdown = true;
        }
        else
        {
            //clear the slots - the queue is empty
            ClearSlots();
        }
        
        transform.gameObject.SetActive(true);
    }

    private void InitializeSlots<T>(List<T> itemsQueue)
        where T : Producible
    {
        //go through each item in the queue
        for (int i = 0; i < slots.Count && i < itemsQueue.Count; i++)
        {
            //initialize the slot with item icon
            slots[i].transform.Find("Icon").GetComponent<Image>().sprite = itemsQueue[i].Icon;
        }
    }

    private void ClearSlots()
    {
        //set the timer text
        timeLeftText.text = "Empty";
        for (int i = 0; i < slots.Count; i++)
        {
            //nullify the slot image
            slots[i].transform.Find("Icon").GetComponent<Image>().sprite = null;
        }
    }

    private void FixedUpdate()
    {
        //if the UI is active and the timer is running
        if (countdown)
        {
            //display the time left text
            timeLeftText.text = timer.DisplayTime();
        }
    }

    private void OnDisable()
    {
        caller = null;
        timer = null;
        countdown = false;
    }
}
