using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public string ID;
    //the name of the process
    public string Name { get; private set; }
    
    //if the timer is working
    public bool isRunning { get; private set; }
    //time when timer starts
    public DateTime startTime;
    //time for the timer to finish (like 2h 3min)
    public TimeSpan timeToFinish { get; private set; }
    //time the timer finishes
    public DateTime finishTime;
    //event that is invoked when the timer is finished
    public UnityEvent TimerFinishedEvent;
    
    //seconds left to timer finish
    public double secondsLeft { get; private set; }

    

    //amount (in crystals) to skip the timer
    public int skipAmount
    {
        get
        {
            //calculate the currenct skip amount - min * 2
            return (int) (secondsLeft / 60) * 2;
        }
    }

    /*
     * Initializes the timer
     * @processName - process name
     * @start - timer start DateTime
     * @time - time it takes to finish
     */
    public void Initialize(string processName, DateTime start, TimeSpan time)
    {
        //initialize fields and properties
        Name = processName;

        startTime = start;
        timeToFinish = time;
        finishTime = start.Add(time);

        TimerFinishedEvent = new UnityEvent();
    }

    public void StartTimer()
    {
        //initialize total second timer has left to run
        secondsLeft = timeToFinish.TotalSeconds;
        //timer is running, set the bool to true
        isRunning = true;
    }

    private void Update()
    {
        //if the timer is working
        if (isRunning)
        {
            //if there is time left
            if (secondsLeft > 0)
            {
                //decrease time left
                secondsLeft -= Time.deltaTime;
            }
            else
            {
                //reset seconds left
                secondsLeft = 0;
                //timer is not running
                isRunning = false;
                //timer finished - invoke event
                TimerFinishedEvent.Invoke();
            }
        }
    }
    public DateTime dateTimeOfPause;
    private void OnApplicationPause(bool isPaused)
    {
        if (isRunning)
        {
            if (isPaused)
            {
                Debug.Log("Game paused");
                dateTimeOfPause = DateTime.Now;
            }
            else
            {
                Debug.Log("Unpused");
                secondsLeft -= (DateTime.Now - dateTimeOfPause).TotalSeconds;
            }
        }
    }
    public DateTime dateTimeOfDisable;
    private void OnDisable()
    {
        if (isRunning)
        {
            dateTimeOfDisable = DateTime.Now;
            Debug.Log("Disablet at: " + dateTimeOfDisable);
        }
    }
    private void OnEnable()
    {
        if (isRunning)
        {
            secondsLeft -= (DateTime.Now - dateTimeOfDisable).TotalSeconds;
            Debug.Log("Resumed after: " + (DateTime.Now - dateTimeOfDisable).TotalSeconds);
        }
    }
    /*
     * @return time text string in format of 2 numbers
     * For example 1h 2min or 3min 25sec
     */
    public string DisplayTime()
    {
        string text = "";
        TimeSpan timeLeft = TimeSpan.FromSeconds(secondsLeft);

        //if there are days left to finish the process
        if (timeLeft.Days != 0)
        {
            //Example 1d 3h
            text += timeLeft.Days + "d ";
            text += timeLeft.Hours + "h";
        }
        //if there are hours left to finish the process
        else if (timeLeft.Hours != 0)
        {
            //Example 2h 15min
            text += timeLeft.Hours + "h ";
            text += timeLeft.Minutes + "min";
        }
        //if there are minutes left to finish the process 
        else if (timeLeft.Minutes != 0)
        {
            //Example 3min 34sec
            text += timeLeft.Minutes + "min ";
            text += timeLeft.Seconds + "sec";
        }
        //if there are seconds left to finish the process
        else if (secondsLeft > 0)
        {
            //Example 45sec
            text += Mathf.FloorToInt((float) secondsLeft) + "sec";
        }
        //no time left - timer is finished
        else
        {
            text = "Finished";
        }

        //return formatted string
        return text;
    }

    /*
     * Skip timer before tjhe finishing time
     */
    public void SkipTimer()
    {
        //reset seconds left
        secondsLeft = 0;
        //set the finish time to now
        finishTime = DateTime.Now;
    }
}
