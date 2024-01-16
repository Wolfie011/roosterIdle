using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour
{
    //fields for all the elements
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private Transform goalsContent;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI coinsText;
    private Quest currQuest;
    public GameObject questOBJ;
    public void Initialize(Quest quest)
    {
        titleText.text = quest.Information.Name;
        descriptionText.text = quest.Information.Description;
        currQuest = quest;

        int childCount = goalsContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(goalsContent.GetChild(i).gameObject);
        }
        foreach (var goal in quest.Goals)
        {
            GameObject goalObj = Instantiate(goalPrefab, goalsContent);
            goalObj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = goal.GetDescription();

            GameObject countObj = goalObj.transform.Find("Count").gameObject;
            GameObject skipObj = goalObj.transform.Find("Skip").gameObject;

            if (goal.Completed)
            {
                countObj.SetActive(false);
                skipObj.SetActive(false);
                goalObj.transform.Find("Done").gameObject.SetActive(true);
            }
            else
            {
                countObj.GetComponent<TextMeshProUGUI>().text = goal.CurrentAmount + "/" + goal.RequiredAmount;
                
                skipObj.GetComponent<Button>().onClick.AddListener(delegate
                {
                    goal.Skip();
                    
                    countObj.SetActive(false);
                    skipObj.SetActive(false);
                    goalObj.transform.Find("Done").gameObject.SetActive(true);
                });
            }
        }

        xpText.text = quest.Reward.XP.ToString();
        coinsText.text = quest.Reward.Currency.ToString();
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
        int childCount = goalsContent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(goalsContent.GetChild(i).gameObject);
        }
    }
    public void DestroyAfterOpenOnComplete()
    {
        if(currQuest.Completed)
        {
            CloseWindow();
            Destroy(questOBJ.gameObject);
        }
        else CloseWindow();
    }
}
