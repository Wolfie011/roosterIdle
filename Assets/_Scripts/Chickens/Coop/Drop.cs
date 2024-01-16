using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Drop : MonoBehaviour
{
    public Product drop;
    public Timer timer;
    public Slider progress;
    public DropStorage dropStorage;
    public CoopUI coopUI;

    private void Start()
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = drop.Icon;
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = drop.Name;
        gameObject.transform.Find("Rate").GetComponent<TextMeshProUGUI>().text = $"{drop.AquiredAmount} / {drop.productionTime.TotalSeconds} s";
        InitTimer();
    }

    public void InitTimer()
    {
        timer = gameObject.AddComponent<Timer>();
        timer.Initialize(SaveData.GenerateUUID(), DateTime.Now, drop.productionTime);
        timer.TimerFinishedEvent.AddListener(delegate
        {
            //StorageManager.current.UpdateItems(drop.ItemsAquired, true);
            if (coopUI.storageCoop.ContainsKey(drop))
            {
                coopUI.storageCoop[drop] += drop.AquiredAmount;
            }
            else
            {
                coopUI.storageCoop.Add(drop, drop.AquiredAmount);
            }
            dropStorage.updateVisual();
            Debug.Log("Finished go next");
            Destroy(timer);
            timer = null;
            InitTimer();
        });
        timer.StartTimer();
    }
    private void FixedUpdate()
    {
        progress.value = (float)(1.0 - timer.secondsLeft / timer.timeToFinish.TotalSeconds);
    }

}
