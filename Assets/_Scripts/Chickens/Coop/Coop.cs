using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Coop : Building
{
    public Coop instance;
    public CoopUI coopUI;
    public bool isStructNew = true;

    [SerializeField] private GameObject windowPrefab;

    public CoopSave dataCOP = new CoopSave();

    private void Awake()
    {
        instance = this;
        Initialize();
    }
    public void InitializeOnLoad(CoopSave coopSave)
    {
        isStructNew = false;
        dataCOP = coopSave;
        coopUI.slot.InitializeOnLoad(coopSave.assetID);
    }
    public void Initialize()
    {
        //instantiate UI
        GameObject window = Instantiate(windowPrefab, GameManager.current.canvas.transform);
        window.GetComponent<RectTransform>().transform.position = new Vector3(Screen.width/2, (Screen.height + (Screen.height / 2)), 0);
        //make it invisible
        //window.SetActive(false);

        //get the hatcher ui script
        coopUI = window.transform.Find("CoopUI").GetComponent<CoopUI>();

        coopUI.parentCoop = instance;

        //initialize the UI
        coopUI.Initialize();
    }
    
    public virtual void onClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //initialize coop UI so it is updated
        coopUI.Initialize();
        //make the UI visible after the click on the building
        //coopUI.gameObject.SetActive(true);
        if (GridBuildingSystem.current.temp == null)
        {
            coopUI.openWindow();
        }
    }
    private void newStruct()
    {
        dataCOP.assetID = null;
    }
    private void OnMouseUpAsButton()
    {
        onClick();
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
        if(isStructNew)
        {
            newStruct();
        }
        coopUI.getItems();
        dataCOP.ID = data.ID;
        dataCOP.assetID = coopUI.slot.chickenKey;
        GameManager.current.saveData.AddData(dataCOP);
    }
}
