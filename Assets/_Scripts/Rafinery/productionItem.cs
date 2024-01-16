using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class productionItem : MonoBehaviour
{
    public RafineryUI rafineryUI;
    public Producible producible;
    public Slider progressBar;

    public bool countdown;

    private void Start()
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = producible.Icon;
        gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{producible.Name}";
    }
    public void FixedUpdate()
    {
        if (countdown)
        {
            progressBar.value = (float)(1.0 - rafineryUI.timer.secondsLeft / rafineryUI.timer.timeToFinish.TotalSeconds);
        }
    }
}
