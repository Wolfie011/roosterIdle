using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Breeder : Building
{
    public Breeder instance;
    public BreederUI breederUI;
    public bool isStructNew = true;

    [SerializeField] private GameObject windowPrefab;

    public BreederSave dataBRD = new BreederSave();

    private void Awake()
    {
        instance = this;
        Initialize();
    }
    public void InitializeOnLoad(BreederSave breederSave)
    {
        isStructNew = false;
        dataBRD = breederSave;
        for (int i = 0; i < 2; i++)
        {
            breederUI.slots[i].index = i;
            breederUI.slots[i].InitializeOnLoad(breederSave.slotAssetName[i]);
        }
        
        //breederUI.checkChickens();
        breederUI.InitializeOnLoad(breederSave);
    }
    public void Initialize()
    {
        GameObject window = Instantiate(windowPrefab, GameManager.current.canvas.transform);

        //make it invisible
        window.GetComponent<RectTransform>().transform.position = new Vector3(Screen.width / 2, (Screen.height + (Screen.height / 2)), 0);

        //get the hatcher ui script
        breederUI = window.transform.Find("BreederUI").GetComponent<BreederUI>();

        breederUI.parentBreeder = instance;

        //initialize the UI
        breederUI.Initialize();
    }
    public virtual void onClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //initialize breeder UI so it is updated
        breederUI.Initialize();
        //make the UI visible after the click on the building
        if (GridBuildingSystem.current.temp == null)
        {
            breederUI.openWindow();
        }
    }
    private void OnMouseUpAsButton()
    {
        onClick();
    }
    public void newStruct()
    {
        for(int i = 0; i < breederUI.slots.Count; i++)
        {
            dataBRD.slotAssetName.Add(null);

            TimerData TM = new TimerData();
            TM.startTime = new DateTime(2000, 10, 10, 10, 20, 30);
            TM.secondsLeft = 0;
            TM.finishTime = new DateTime(2000, 10, 10, 10, 20, 30);
            dataBRD.timerData = TM;
        }
    }
    private void OnApplicationQuit()
    {
        if(Placed)
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

        if (isStructNew)
        {
            newStruct();
        }
        for (int i = 0; i < breederUI.slots.Count; i++)
        {
            if (breederUI.slots[i].chickenKey == null) dataBRD.slotAssetName[i] = null;
            else dataBRD.slotAssetName[i] = breederUI.slots[i].chickenKey;
            
        }
        if (!breederUI.GetComponent<Timer>())
        {
            if (breederUI.tempEgg != null)
            {

            }
            else
            {
                Debug.Log("Timer wyebany");
                dataBRD.timerData.startTime = new DateTime(2000, 10, 10, 10, 20, 30);
                dataBRD.timerData.secondsLeft = 0;
                dataBRD.timerData.finishTime = new DateTime(2000, 10, 10, 10, 20, 30);
                dataBRD.timerData.pauzeTime = DateTime.Now;
            }
        }
        else
        {
            dataBRD.timerData.startTime = breederUI.GetComponent<Timer>().startTime;
            dataBRD.timerData.secondsLeft = breederUI.GetComponent<Timer>().secondsLeft;
            dataBRD.timerData.finishTime = breederUI.GetComponent<Timer>().finishTime;
            dataBRD.timerData.pauzeTime = DateTime.Now;
        }
        dataBRD.ID = data.ID;
        GameManager.current.saveData.AddData(dataBRD);
    }

}
