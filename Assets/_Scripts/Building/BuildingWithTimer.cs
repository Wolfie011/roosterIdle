using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BuildingWithTimer : Building
{
    public int builtTimeInMinutes;
    
    public override void Place()
    {
        if(!built & !building)
        {
            building = true;
            Timer timer = gameObject.AddComponent<Timer>();
            timer.Initialize("Building", DateTime.Now, TimeSpan.FromSeconds(builtTimeInMinutes));
            timer.StartTimer();
            timer.TimerFinishedEvent.AddListener(delegate
            {
                built = true;
                building = false;
                Destroy(timer);
            });
        }
        
        base.Place();
    }
    protected override void OnClick()
    {
        if (gameObject.GetComponent<Timer>())
        {
            TimerTooltip.ShowTimer_Static(gameObject);
        }
    }
    private void OnMouseUpAsButton()
    {
        OnClick();
    }
    private void OnApplicationQuit()
    {
        if (Placed)
        {
            //updating the position of the building for saving
            data.position = transform.position;
            data.area = area;
            //Adding the data to the main save object
            GameManager.current.saveData.AddData(data);
        }
        if (gameObject.GetComponent<Timer>() && gameObject.GetComponent<Timer>().secondsLeft != 0)
        {
            dataTM.ID = data.ID;
            dataTM.Name = gameObject.GetComponent<Timer>().Name;
            dataTM.startTime = gameObject.GetComponent<Timer>().startTime;
            dataTM.timeToFinish = gameObject.GetComponent<Timer>().timeToFinish;
            dataTM.finishTime = gameObject.GetComponent<Timer>().finishTime;
            dataTM.secondsLeft = gameObject.GetComponent<Timer>().secondsLeft;
            GameManager.current.saveData.AddData(dataTM);
        }
    }
}
