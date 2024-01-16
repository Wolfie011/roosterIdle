using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HatcherSlot : MonoBehaviour
{
    public HatcherUI hatcherUI;
    public int price;
    public CurrencyType type;
    public Slider progressSlider;
    public int index;
    public bool status;

    public Egg egg;
    public string eggKey;

    private Timer timer;
    public bool countdown = false;
    public TextMeshProUGUI timeLeftText;

    private void Awake()
    {
        gameObject.transform.Find("unlockBTN").gameObject.transform.Find("slotPrice")
            .GetComponent<TextMeshProUGUI>().text = price.ToString();
        gameObject.transform.Find("unlockBTN").gameObject.transform.Find("slotPrice")
            .gameObject.transform.Find("slotCurrency").GetComponent<Image>().sprite = ShopManager.currencySprites[type];
        CheckOnLoad();
    }
    public void InitializeOnLoad(bool isStatus, string assetName, string assetID, TimerData TMData)
    {
        status = isStatus;
        if(status)
        {
            gameObject.transform.Find("choseBTN").gameObject.SetActive(true);
            gameObject.transform.Find("unlockBTN").gameObject.SetActive(false);

            //Debug.Log("Slot is unlocked ...");

            if(!string.IsNullOrEmpty(assetID))
            {
                egg = StorageManager.current.eggs[assetID];
                eggKey = assetID;

                //CheckTimer
                if (TMData.timeToFinish.Equals(TMData.startTime)) Debug.Log("ERR OCCURED");
                else
                {
                    gameObject.transform.Find("choseBTN").gameObject.SetActive(false);
                    gameObject.transform.Find("unlockBTN").gameObject.SetActive(false);
                    gameObject.transform.Find("HatchItem").gameObject.SetActive(true);

                    if (TMData.finishTime > DateTime.Now)
                    {
                        double secondsL = TMData.secondsLeft - (DateTime.Now - TMData.pauzeTime).TotalSeconds;

                        timer = gameObject.AddComponent<Timer>();

                        timer.dateTimeOfPause = TMData.pauzeTime;
                        timer.dateTimeOfDisable = TMData.pauzeTime;

                        Debug.Log("WCZYTANIE CZASU JAJKA: " + secondsL);

                        timer.Initialize($"{eggKey}", DateTime.Now, TimeSpan.FromSeconds(secondsL));
                        timer.TimerFinishedEvent.AddListener(delegate
                        {
                            Debug.Log("egg finished added by save!");
                            Debug.Log(timer);
                            Finisher();
                            countdown = false;
                            Destroy(timer);
                        });
                        timer.StartTimer();
                        countdown = true;
                        FixedUpdate();
                        Debug.Log(timer.secondsLeft);
                    }
                    else
                    {
                        //Ready for collect
                        //Debug.Log("... egg ready for collection!");

                        gameObject.transform.Find("HatchItem").transform.Find("Progress").gameObject.SetActive(false);
                        gameObject.transform.Find("HatchItem").transform.Find("HatchFinish").gameObject.SetActive(true);
                    }
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (countdown)
        {
            progressSlider.value = (float)(1.0 - timer.secondsLeft / timer.timeToFinish.TotalSeconds);
            timeLeftText.text = timer.DisplayTime();
        }
    }
    public void Collect()
    {
        Chicken newChicken = Instantiate(egg.hatchedChicken);

        newChicken.Strength = egg.Strength;
        newChicken.Growth = egg.Growth;
        newChicken.Gain = egg.Gain;
        newChicken.inUse = false;
        newChicken.ID = eggKey;
        newChicken.breedTime = egg.hatchedChicken.breedTime;

        StorageManager.current.chickens.Add(eggKey, newChicken);
        StorageManager.current.eggs.Remove(eggKey);
        GameManager.current.saveData.RemoveSpecial(eggKey, 0);

        egg = null;
        eggKey = null;
        hatcherUI.parentHatcher.dataHTC.slotAssetID[index] = null;
        CheckOnLoad();
    }
    public void CheckOnLoad()
    {
        if (status)
        {
            if (egg != null)
            {
                gameObject.transform.Find("choseBTN").gameObject.SetActive(false);
                gameObject.transform.Find("unlockBTN").gameObject.SetActive(false);
                gameObject.transform.Find("HatchItem").gameObject.SetActive(true);

                Transform hatch = gameObject.transform.Find("HatchItem").gameObject.transform;
                hatch.Find("Icon").GetComponent<Image>().sprite = egg.Icon;
                hatch.Find("Name").GetComponent<TextMeshProUGUI>().text = egg.Name;
            }
            else
            {
                gameObject.transform.Find("choseBTN").gameObject.SetActive(true);
                gameObject.transform.Find("unlockBTN").gameObject.SetActive(false);
                gameObject.transform.Find("HatchItem").gameObject.SetActive(false);

                gameObject.transform.Find("HatchItem").transform.Find("Progress").gameObject.SetActive(true);
                gameObject.transform.Find("HatchItem").transform.Find("HatchFinish").gameObject.SetActive(false);
            }

            if (gameObject.GetComponent<Timer>())
            {
                gameObject.transform.Find("HatchItem").transform.Find("Progress").gameObject.SetActive(true);
                gameObject.transform.Find("HatchItem").transform.Find("HatchFinish").gameObject.SetActive(false);
            }
            else
            {
                gameObject.transform.Find("HatchItem").transform.Find("Progress").gameObject.SetActive(false);
                gameObject.transform.Find("HatchItem").transform.Find("HatchFinish").gameObject.SetActive(true);
            }
        } 
    }
    public void Finisher()
    {
        gameObject.transform.Find("HatchItem").transform.Find("Progress").gameObject.SetActive(false);
        gameObject.transform.Find("HatchItem").transform.Find("HatchFinish").gameObject.SetActive(true);
    }
    public void TimerInit()
    {

        //Debug.Log("EGG SELECTED, TIMER ADDED BY HAND");
        timer = gameObject.AddComponent<Timer>();
        Debug.Log(egg.productionTime);
        timer.Initialize("HatchData", DateTime.Now, egg.productionTime);
        timer.TimerFinishedEvent.AddListener(delegate
        {
            Debug.Log("egg finished orgin init!");
            Finisher();
            countdown = false;
            Destroy(timer);
        });
        timer.StartTimer();
        countdown = true;
        FixedUpdate();
    }
    public void EggSelection(HatcherSlot thisSlot)
    {
        hatcherUI.InitializeEggs(thisSlot);
    }
    public void UnlockSlot()
    {
        EventManager.Instance.AddListenerOnce<EnoughCurrencyGameEvent>(OnEnoughCurrency);
        EventManager.Instance.AddListenerOnce<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);
        CurrencyChangeGameEvent info = new CurrencyChangeGameEvent(-price, type);
        EventManager.Instance.QueueEvent(info);
    }
    private void OnEnoughCurrency(EnoughCurrencyGameEvent info)
    {
        EventManager.Instance.RemoveListener<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);
        gameObject.GetComponent<HatcherSlot>().status = true;
        gameObject.transform.Find("choseBTN").gameObject.SetActive(true);
        gameObject.transform.Find("unlockBTN").gameObject.SetActive(false);
    }
    private void OnNotEnoughCurrency(NotEnoughCurrencyGameEvent info)
    {
        EventManager.Instance.RemoveListener<EnoughCurrencyGameEvent>(OnEnoughCurrency);
    }
}
