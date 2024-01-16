using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hatcher : Building
{
    public HatcherUI hatcherUI;
    public Hatcher instance;
    public bool isStructNew = true;

    [SerializeField] private GameObject windowPrefab;

    public HatcherSave dataHTC = new HatcherSave();

    private void Awake()
    {
        instance = this;
        Initialize();
    }
    public void InitializeOnLoad(HatcherSave hatcherSave)
    {
        isStructNew = false;
        dataHTC = hatcherSave;
        for (int i = 0; i < 4; i++)
        {
            hatcherUI.slots[i].index = i;
            hatcherUI.slots[i].InitializeOnLoad(hatcherSave.slotStatus[i], hatcherSave.slotAssetName[i], hatcherSave.slotAssetID[i], hatcherSave.timerDatas[i]);
        }
    }
    public void Initialize()
    {
        //instantiate UI
        GameObject window = Instantiate(windowPrefab, GameManager.current.canvas.transform);

        //make it invisible
        window.GetComponent<RectTransform>().transform.position = new Vector3(Screen.width / 2, (Screen.height + (Screen.height / 2)), 0);

        //get the hatcher ui script
        hatcherUI = window.transform.Find("HatcherUI").GetComponent<HatcherUI>();

        hatcherUI.parentHatcher = instance;

        //initialize the UI
        hatcherUI.Initialize();
    }
    public void newStruct()
    {
        Debug.Log("FILLING EMPTY ARRAYS");
        for (int i = 0; i < hatcherUI.slots.Count; i++)
        {
            dataHTC.slotStatus.Add(false);
            dataHTC.slotAssetName.Add(null);
            dataHTC.slotAssetID.Add(null);
            dataHTC.isCountdown.Add(false);

            TimerData TM = new TimerData();
            TM.startTime = new DateTime(2000, 10, 10, 10, 20, 30);
            TM.secondsLeft = 0;
            TM.finishTime = new DateTime(2000, 10, 10, 10, 20, 30);
            dataHTC.timerDatas.Add(TM);
        }
    }
    public virtual void onClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //initialize hatcher UI so it is updated
        hatcherUI.Initialize();
        //make the UI visible after the click on the building
        if(GridBuildingSystem.current.temp == null)
        {
            hatcherUI.openWindow();
        }
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

        if (isStructNew)
        {
            //Debug.Log("Initializing new data for hatcher");
            newStruct();
        }

        for (int i = 0; i < hatcherUI.slots.Count; i++)
        {
            dataHTC.slotStatus[i] = hatcherUI.slots[i].status;

            if (hatcherUI.slots[i].egg == null) dataHTC.slotAssetName[i] = null;
            else
            {
                dataHTC.slotAssetID[i] = hatcherUI.slots[i].eggKey;
            }

            if (hatcherUI.slots[i].GetComponent<Timer>())
            {
                dataHTC.timerDatas[i].startTime = hatcherUI.slots[i].GetComponent<Timer>().startTime;
                dataHTC.timerDatas[i].secondsLeft = hatcherUI.slots[i].GetComponent<Timer>().secondsLeft;
                dataHTC.timerDatas[i].finishTime = hatcherUI.slots[i].GetComponent<Timer>().finishTime;
                dataHTC.timerDatas[i].pauzeTime = DateTime.Now;
                dataHTC.isCountdown[i] = true;
            }
            else
            {
                dataHTC.timerDatas[i].startTime = new DateTime(2000, 10, 10, 10, 20, 30);
                dataHTC.timerDatas[i].secondsLeft = 0;
                dataHTC.timerDatas[i].finishTime = new DateTime(2000, 10, 10, 10, 20, 30);
                dataHTC.timerDatas[i].pauzeTime = new DateTime(2000, 10, 10, 10, 20, 30);
                dataHTC.isCountdown[i] = false;
            }
        }
        dataHTC.ID = data.ID;
        
        GameManager.current.saveData.AddData(dataHTC);
    }
}
