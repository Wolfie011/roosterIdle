using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public ShopItem item;
    public buildType buildType;
    public bool Placed;
    public bool isNew = true;
    public bool isMoveDisabled;
    public BoundsInt area;

    private Vector3 origin;

    [ReadOnly] public PlaceableObjectData data = new PlaceableObjectData();
    [ReadOnly] public TimerData dataTM = new TimerData();

    public bool built;
    public bool building;

    #region Build Method
    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;
        if (GridBuildingSystem.current.CanTakeArea(areaTemp))
        {
            return true;
        }
        return false;
    }
    public virtual void Place()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        Placed = true;
        origin = transform.position;

        GridBuildingSystem.current.TakeArea(areaTemp);

        GridBuildingSystem.current.temp = null;
        GridBuildingSystem.current.prevArea = new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(1, 2, 1));
        GridBuildingSystem.current.prevPos = new Vector3(0, 0, 0);

        ShopManager.current.buildingPanel.SetActive(false);
    }

    #endregion

    #region SaveMethods

    //Initialization method used for first placement (it wasn't saved before)
    public void Initialize(ShopItem shopItem)
    {
        item = shopItem;
        data.assetName = item.Name;
        data.ID = SaveData.GenerateUUID();
        data.built = false;
        data.building = false;
        //area = data.area;
    }

    //Initialization method used for loading the object after it's been saved
    public void Initialize(ShopItem shopItem, PlaceableObjectData objectData)
    {
        item = shopItem;
        data = objectData;
        area = data.area;
        building = data.building;
        built = data.built;
    }
    public void InitializeTimerOnLoad(TimerData timerData, bool isExpired)
    {
        dataTM = timerData;
        double secondsL = timerData.secondsLeft - (DateTime.Now - timerData.pauzeTime).TotalSeconds;
        if (isExpired)
        {
            GameManager.current.saveData.RemoveData(dataTM);
            built = true;
            building = false;
        }
        else
        {
            building = true;
            Timer timer = gameObject.AddComponent<Timer>();
            timer.Initialize(dataTM.Name, DateTime.Now, TimeSpan.FromSeconds(dataTM.secondsLeft));
            timer.StartTimer();
            timer.TimerFinishedEvent.AddListener(delegate
            {
                GameManager.current.saveData.RemoveData(dataTM);
                built = true;
                building = false;
                Destroy(timer);
            });
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
        GameManager.current.saveData.AddData(data);
    }
    #endregion

    #region Building Mod

    private float time = 0f;
    private bool touching;
    private bool moving;

    private void Update()
    {
        if (touching && Input.GetMouseButton(0) && GridBuildingSystem.current.temp == null && !isMoveDisabled)
        {
            if (PanZoom.current.moveAllowed) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            //increase time elapsed
            time += Time.deltaTime;

            //time limit exceeded
            if (time > 3f)
            {
                TimerTooltip.HideTimer_Static();

                touching = false;
                Debug.Log(gameObject.GetComponent<Building>().name);
                Placed = false;
                GridBuildingSystem.current.temp = gameObject.GetComponent<Building>();
                GridBuildingSystem.current.prevArea = area;
                GridBuildingSystem.current.ClearAreaOnMove();
                ShopManager.current.buildingPanel.SetActive(true);
            }
        }
    }

    private void OnMouseDown()
    {
        time = 0;
        touching = true;
    }

    protected virtual void OnClick() { }

    private void OnMouseUpAsButton()
    {
        if (moving)
        {
            moving = false;
            return;
        }

        OnClick();
    }
    #endregion
}
public enum buildType
{
    Production,
    Chicken,
    Storage,
}
