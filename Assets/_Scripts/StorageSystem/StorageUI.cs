using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    //action that is going to be invoked when 
    private Action increaseAction;

    //UI fields
    [SerializeField] private TextMeshProUGUI storageTypeText;
    [SerializeField] private TextMeshProUGUI maxItemsText;
    [SerializeField] private Slider maxItemsSlider;

    [SerializeField] private GameObject itemsView;
    [SerializeField] private GameObject increaseView;

    [SerializeField] private Transform itemsContent;
    [SerializeField] private Transform increaseContent;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject customePrefab;

    /*
     * Display the storage building name
     */
    public void SetNameText(string name)
    {
        storageTypeText.text = name;
    }

    /*
     * Initialize the UI
     */
    public void Initialize(int currentAmount, int maxAmount, Dictionary<CollectibleItem, int> itemAmounts,
                            Dictionary<CollectibleItem, int> tools, Action onIncrease)
    {
        //set the capacity text
        maxItemsText.text = currentAmount + "/" + maxAmount;
        //set the capacity slider value
        maxItemsSlider.value = (float)currentAmount / maxAmount;

        //initialize the items view
        if(storageTypeText.text == "Barn")
        {
            InitializeBarn();
            InitializeTools(tools);
        }
        else
        {
            InitializeItems(itemAmounts);
            //initialize the tools view
            InitializeTools(tools);
        }
        //set the action
        increaseAction = onIncrease;
    }

    /*
     * Initialize the items view
     */
    private void InitializeBarn()
    {
        int childCount = itemsContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(itemsContent.GetChild(i).gameObject);
        }

        if (StorageManager.current.chickens == null) { }
        else
        {
            //go through each item
            foreach (var chicken in StorageManager.current.chickens.Values)
            {
                if (chicken.inUse)
                {
                    GameObject chickenHolder = Instantiate(customePrefab, itemsContent);
                    chickenHolder.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = chicken.Name;
                    chickenHolder.transform.Find("Icon").GetComponent<Image>().sprite = chicken.Icon;
                    chickenHolder.transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = $"{chicken.Strength} {chicken.Growth} {chicken.Gain}";
                    chickenHolder.transform.Find("block").gameObject.SetActive(true);
                }
                else
                {
                    GameObject chickenHolder = Instantiate(customePrefab, itemsContent);
                    chickenHolder.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = chicken.Name;
                    chickenHolder.transform.Find("Icon").GetComponent<Image>().sprite = chicken.Icon;
                    chickenHolder.transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = $"{chicken.Strength} {chicken.Growth} {chicken.Gain}";
                } 
            }
        }
        if (StorageManager.current.eggs == null) { }
        else
        {
            //go through each item
            foreach (var egg in StorageManager.current.eggs.Values)
            {
                if (egg.inUse)
                {
                    GameObject chickenHolder = Instantiate(customePrefab, itemsContent);
                    chickenHolder.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = egg.Name;
                    chickenHolder.transform.Find("Icon").GetComponent<Image>().sprite = egg.Icon;
                    chickenHolder.transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = $"{egg.Strength} {egg.Growth} {egg.Gain}";
                    chickenHolder.transform.Find("block").gameObject.SetActive(true);
                }
                else
                {
                    GameObject chickenHolder = Instantiate(customePrefab, itemsContent);
                    chickenHolder.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = egg.Name;
                    chickenHolder.transform.Find("Icon").GetComponent<Image>().sprite = egg.Icon;
                    chickenHolder.transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = $"{egg.Strength} {egg.Growth} {egg.Gain}";
                }
                
            }
        }
    }
    private void InitializeItems(Dictionary<CollectibleItem, int> itemAmounts)
    {
        //if the window was initialized before -> clear
        int childCount = itemsContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(itemsContent.GetChild(i).gameObject);
        }

        if (itemAmounts == null)
        {

        }
        else
        {
            //go through each item
            foreach (var itemPair in itemAmounts)
            {
                //Debug.Log(itemPair.Key + " has " + itemPair.Value);

                if (itemPair.Value == 0)
                {
                    
                }
                else
                {
                    //instantiate an item holder
                    GameObject itemHolder = Instantiate(itemPrefab, itemsContent);
                    //set the icon image
                    itemHolder.transform.Find("Icon").GetComponent<Image>().sprite = itemPair.Key.Icon;
                    //set the amount text
                    itemHolder.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = itemPair.Value.ToString();
                }
            }
        }
    }

    /*
     * Initialize the tools view
     */
    private void InitializeTools(Dictionary<CollectibleItem, int> tools)
    {
        //initialize the counter
        int i = 0;
        if (tools == null)
        {

        }
        else
        {
            //go through each tool in the dictionary
            foreach (var itemPair in tools)
            {
                //get the tool holder UI
                GameObject itemHolder = increaseContent.GetChild(i).gameObject;
                //initialize icon image
                itemHolder.transform.Find("Icon").GetComponent<Image>().sprite = itemPair.Key.Icon;
                //initialize amount text in a format "have/needed" (3/4)
                itemHolder.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text =
                    StorageManager.current.GetAmount(itemPair.Key) + "/" + itemPair.Value;
                //increase the counter
                i++;
            }
        }
    }

    #region Buttons

    //close the storage window
    public void CloseButton_Click()
    {
        gameObject.SetActive(false);
    }

    //open increase window
    public void IncreaseButton_Click()
    {
        increaseView.SetActive(true);
    }

    //confirm increase
    public void ConfirmButton_Click()
    {
        increaseAction.Invoke();
    }

    //return to items view from increase window
    public void BackButton_Click()
    {
        increaseView.SetActive(false);
    }

    #endregion
}
