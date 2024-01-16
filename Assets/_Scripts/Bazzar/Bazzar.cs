using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bazzar : BuildingWithTimer
{
    public BazzarUI bazzarUI;
    public Bazzar instance;
    [SerializeField] private GameObject windowPrefab;

    private void Awake()
    {
        instance = this;
        Initialize();
    }
    public void Initialize()
    {
        //instantiate UI
        GameObject window = Instantiate(windowPrefab, GameManager.current.canvas.transform);

        //make it invisible
        window.SetActive(false);

        //get the hatcher ui script
        bazzarUI = window.GetComponent<BazzarUI>();

        bazzarUI.parentBazzar = instance;

        //initialize the UI
        bazzarUI.Initialize();
    }
    protected override void OnClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (built)
        {
            //initialize hatcher UI so it is updated
            bazzarUI.Initialize();
            //make the UI visible after the click on the building
            bazzarUI.gameObject.SetActive(true);
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
    }
}
