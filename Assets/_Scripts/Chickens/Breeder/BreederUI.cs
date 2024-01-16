using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BreederUI : MonoBehaviour
{
    public Breeder parentBreeder;
    public List<BreederSlot> slots;

    [SerializeField] private GameObject breederView;
    [SerializeField] private GameObject eggView;

    [SerializeField] private Transform eggContent;

    [SerializeField] private Transform progressContent;
    [SerializeField] private Transform dropContent;

    public GameObject eggPrefab;

    public Timer timer;
    public bool countdown;
    public Egg tempEgg;

    private RectTransform rt;
    private RectTransform prt;
    public bool isVisible = false;

    System.Random rnd = new System.Random();
    public void Initialize()
    {
        InitializeSlots();
        BreederWindow_Click();
    }
    private void Awake()
    {
        rt = gameObject.transform.parent.GetComponent<RectTransform>();
        prt = gameObject.transform.parent.GetComponentInParent<RectTransform>();
    }
    public void checkBreed()
    {
        if (slots[0].gameObject.GetComponent<BreederSlot>().chicken != null
            && slots[1].gameObject.GetComponent<BreederSlot>().chicken != null)
        {
            if (!gameObject.GetComponent<Timer>())
            {
                startBreed();
            }
        }
    }
    public void openWindow()
    {
        if (!isVisible)
        {
            LeanTween.moveY(rt, 0, 0.2f);
            isVisible = true;
        }
    }
    public void closeWindow()
    {
        LeanTween.moveY(rt, (Screen.height), 0.2f);
        isVisible = false;
    }
    public void startBreed()
    {
        var leftChicken = slots[0].gameObject.GetComponent<BreederSlot>().chicken;
        var rightChicken = slots[1].gameObject.GetComponent<BreederSlot>().chicken;

        progressContent.Find("progress").Find("chances").GetComponent<TextMeshProUGUI>().text =
            $"Chances: {checkChances(leftChicken, rightChicken)}%";

        TimeSpan timeOfBreed = leftChicken.breedTime + rightChicken.breedTime;
        Debug.Log(timeOfBreed);
        timer = gameObject.AddComponent<Timer>();
        timer.Initialize($"{SaveData.GenerateUUID()}", DateTime.Now, timeOfBreed);
        timer.TimerFinishedEvent.AddListener(delegate {

            InitializeDrop(createChicken());

            countdown = false;
            Destroy(timer);

        });
        timer.StartTimer();
        countdown = true;
        FixedUpdate();
    }
    public void InitializeDrop(Egg newEgg)
    {
        tempEgg = newEgg;
        progressContent.gameObject.SetActive(false);
        dropContent.gameObject.SetActive(true);

        var brov = dropContent.Find("Background");
        brov.Find("Icon").GetComponent<Image>().sprite = newEgg.Icon;
        brov.Find("Name").GetComponent<TextMeshProUGUI>().text = newEgg.Name;
        brov.Find("Stats").GetComponent<TextMeshProUGUI>().text = 
            $"S: {newEgg.Strength} / G: {newEgg.Growth} / G: {newEgg.Gain}";
    }
    private Egg createChicken()
    {
        var leftChicken = slots[0].gameObject.GetComponent<BreederSlot>().chicken;
        var rightChicken = slots[1].gameObject.GetComponent<BreederSlot>().chicken;

        Types match = TypperCheck(leftChicken, rightChicken);
        Egg newEgg = null;
        Egg[] eggLoadList = Resources.LoadAll<Egg>("Eggs");

        for(int i = 0; i < eggLoadList.Length; i++)
        {
            if (eggLoadList[i].eggType == match)
            {
                newEgg = Instantiate(eggLoadList[i]);
                newEgg.hatchTime = eggLoadList[i].hatchTime;
                newEgg.productionTime = eggLoadList[i].productionTime;
                newEgg.inUse = false;
            }
        }

        var flor1 = Math.Floor((leftChicken.Strength + rightChicken.Strength) / 1.5);
        newEgg.Strength = rnd.Next(highlow(leftChicken, rightChicken, "Strength").Strength, (int)flor1 + 2);

        var flor2 = Math.Floor((leftChicken.Growth + rightChicken.Growth) / 1.5);
        newEgg.Growth = rnd.Next(highlow(leftChicken, rightChicken, "Growth").Growth, (int)flor2 + 2);

        var flor3 = Math.Floor((leftChicken.Gain + rightChicken.Gain) / 1.5);
        newEgg.Gain = rnd.Next(highlow(leftChicken, rightChicken, "Gain").Gain, (int)flor3 + 2);

        

        return newEgg;
    }
    public void checkChickens()
    {
        if (slots[0].gameObject.GetComponent<BreederSlot>().chicken != null
            && slots[1].gameObject.GetComponent<BreederSlot>().chicken != null)
        {
            var leftChicken = slots[0].gameObject.GetComponent<BreederSlot>().chicken;
            var rightChicken = slots[1].gameObject.GetComponent<BreederSlot>().chicken;

            progressContent.gameObject.SetActive(true);

            progressContent.Find("progress").Find("chances").GetComponent<TextMeshProUGUI>().text =
            $"Chances: {checkChances(leftChicken, rightChicken)}%";
        }
        else
        {
            progressContent.gameObject.SetActive(false);
        }
    }
    public void InitializeSlots()
    {
        foreach (var slot in slots)
        {
            slot.breederUI = this;
            slot.CheckOnLoad();
        }
    }
    public void InitializeEggs(BreederSlot slot)
    {
        int childCount = eggContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(eggContent.GetChild(i).gameObject);
        }

        if (StorageManager.current.chickens == null)
        {
            Debug.Log("No chickens avalible");
        }
        else
        {
            foreach (var chickenT in StorageManager.current.chickens)
            {
                if (chickenT.Value.inUse)
                {

                }
                else
                {
                    //Debug.Log(chicken.Value.Name);
                    SlotEgg chickenHolder = Instantiate(eggPrefab, eggContent).GetComponent<SlotEgg>();
                    chickenHolder.chicken = chickenT.Value;
                    chickenHolder.chickenKey = chickenT.Key;
                    chickenHolder.slot = slot;
                    chickenHolder.breederUI = this;
                }
                
            }
        }
        EggWindow_Click();
    }
    public void EggWindow_Click()
    {
        eggView.SetActive(true);
        breederView.SetActive(false);
    }
    public void BreederWindow_Click()
    {
        breederView.SetActive(true);
        eggView.SetActive(false);
    }

    public Chicken highlow(Chicken leftChicken, Chicken rightChicken, string stat)
    {
        Chicken high = null;
        switch (stat)
        {
            case "Strength":
                if (leftChicken.Strength > rightChicken.Strength) high = leftChicken;
                else high = rightChicken;
                break;

            case "Growth":
                if (leftChicken.Growth > rightChicken.Growth) high = leftChicken;
                else high = rightChicken;
                break;

            case "Gain":
                if (leftChicken.Gain > rightChicken.Gain) high = leftChicken;
                else high = rightChicken;
                break;
        }
        return high;
    }
    public int chances = 0;
    public int checkChances(Chicken leftChicken, Chicken rightChicken)
    {
        string path = "breedChart";
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        string[] lines = textAsset.text.Split('\n');

        for (int i = 1; i < lines.Length - 1; i++)
        {
            string[] columns = lines[i].Split(',');

            string left = columns[0];
            string right = columns[1];

            if ((left == leftChicken.chickenType.ToString() && right == rightChicken.chickenType.ToString())
                || (left == rightChicken.chickenType.ToString() && right == leftChicken.chickenType.ToString()))
            {
                chances = int.Parse(columns[2]);
            }
        }
        return chances;
    }

    Types eggType = Types.Default;
    public Types TypperCheck(Chicken leftChicken, Chicken rightChicken)
    {
        string path = "breedChart";
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        string[] lines = textAsset.text.Split('\n');

        for (int i = 1; i < lines.Length - 1; i++)
        {
            string[] columns = lines[i].Split(',');

            string left = columns[0];
            string right = columns[1];

            if ((left == leftChicken.chickenType.ToString() && right == rightChicken.chickenType.ToString())
                )
            {
                int chance = int.Parse(columns[2]);
                if (rnd.Next(1, 101) <= chance)
                {
                    string output = columns[3];
                    Enum.TryParse(output, out Types result);
                    eggType = result;
                }
                else
                {
                    switch (rnd.Next(1, 3))
                    {
                        case 1:
                            eggType = leftChicken.chickenType;
                            break;
                        case 2:
                            eggType = rightChicken.chickenType;
                            break;
                    }
                }
            }

        }
        if (leftChicken.chickenType == rightChicken.chickenType)
        {
            eggType = leftChicken.chickenType;
        }
        if (eggType == Types.Default)
        {
            Debug.Log("No matching pair found.");
        }
        return eggType;
    }

    private void FixedUpdate()
    {
        if (countdown)
        {
            progressContent.GetChild(0).gameObject.GetComponent<Slider>().value = (float)(1.0 - timer.secondsLeft / timer.timeToFinish.TotalSeconds);
        }
    }
    public void collectEgg()
    {
        string uuiduuid = SaveData.GenerateUUID();
        EggData eggData = new EggData();
        eggData.ID = uuiduuid;
        eggData.assetName = tempEgg.Name;
        eggData.Strength = tempEgg.Strength;
        eggData.Growth = tempEgg.Growth;
        eggData.Gain = tempEgg.Gain;
        eggData.inUse = false;
        GameManager.current.saveData.AddData(eggData);
        StorageManager.current.eggs.Add(uuiduuid, tempEgg);  
        
        tempEgg = null;

        progressContent.gameObject.SetActive(true);
        //progressContent.Find("progress").Find("chances").gameObject.SetActive(false);
        dropContent.gameObject.SetActive(false);
        checkChickens();
    }

    public void InitializeOnLoad(BreederSave breederSave)
    {
        var leftChicken = slots[0].gameObject.GetComponent<BreederSlot>().chicken;
        var rightChicken = slots[1].gameObject.GetComponent<BreederSlot>().chicken;

        double secondsL = breederSave.timerData.secondsLeft - (DateTime.Now - breederSave.timerData.pauzeTime).TotalSeconds;

        /*if (timerData.finishTime > DateTime.Now)
        {
            Debug.Log("JEST GIT");
            timer = gameObject.AddComponent<Timer>();
            timer.Initialize($"{SaveData.GenerateUUID()}", DateTime.Now, TimeSpan.FromSeconds(secondsL));
            timer.TimerFinishedEvent.AddListener(delegate {

                InitializeDrop(createChicken());

                countdown = false;
                Destroy(timer);

            });
            timer.StartTimer();
            countdown = true;
            FixedUpdate();
        }
        else
        {
            if(timerData.startTime == timerData.finishTime)
            {
                //TIMER EMPTY
            }
            else
            {
                Debug.Log("Stary jest");
            }

        }*/

        if (!string.IsNullOrEmpty(breederSave.slotAssetName[0]) && !string.IsNullOrEmpty(breederSave.slotAssetName[1]))
        {
            Debug.Log("Kurczaki są");
            if(breederSave.timerData.startTime == breederSave.timerData.finishTime)
            {
                //Kurczaki są, ale timer nie ruszany
                Debug.Log("Timera nie ma");
            }
            if(breederSave.timerData.finishTime > DateTime.Now)
            {
                progressContent.gameObject.SetActive(true);
                dropContent.gameObject.SetActive(false);
                //Dodaj timer, kurczaki są i in-progress
                timer = gameObject.AddComponent<Timer>();

                timer.dateTimeOfPause = breederSave.timerData.pauzeTime;
                timer.dateTimeOfDisable = breederSave.timerData.pauzeTime;

                timer.Initialize($"{SaveData.GenerateUUID()}", DateTime.Now, TimeSpan.FromSeconds(secondsL));
                timer.TimerFinishedEvent.AddListener(delegate {

                    InitializeDrop(createChicken());

                    countdown = false;
                    Destroy(timer);

                });
                timer.StartTimer();
                countdown = true;
                FixedUpdate();
            }
            else
            {
                //Kurczaki są, timer się skończył jakiś czas temu.
                progressContent.gameObject.SetActive(false);
                dropContent.gameObject.SetActive(true);
                InitializeDrop(createChicken());
            }
        }
    }
}
