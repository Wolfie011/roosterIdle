using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager current;

    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Transform questsContent;
    [SerializeField] private GameObject questHolder;

    [SerializeField] private Button questWindowCloseBTN;

    public List<Quest> CurrentQuests;

    public Dictionary<string, QuestSave> questDataSave = new Dictionary<string, QuestSave>();

    private void Awake()
    {
        current = this;
    }
    public void InitializeOnLoad(Dictionary<string,QuestSave> questSaveData)
    {
        questDataSave = questSaveData;
        modifier();
    }
    public void modifier()
    {
        Quest[] qList = Resources.LoadAll<Quest>($"Quests/{LevelSystem.Level}");

        foreach (Quest quest in qList)
        {
            Quest tempQ = Instantiate(quest);
            //
            foreach (var QuestData in questDataSave)
            {
                if (QuestData.Key == tempQ.Information.QuestID)
                {
                    upperQuestAdder(QuestData.Value.progress, tempQ);
                }
            }
            CurrentQuests.Add(tempQ);
        }
    }
    private Quest upperQuestAdder(List<int> amounts, Quest quesT)
    {
        Debug.Log(quesT.Information.Name);
        for(int i = 0; i < quesT.Goals.Count; i++)
        {
            Debug.Log($"{quesT.Goals[i].GetDescription()}||| {quesT.Goals[i].CurrentAmount} to: " + amounts[i]);
            quesT.Goals[i].CurrentAmount = amounts[i];
            Debug.Log($"{quesT.Goals[i].GetDescription()} now is equal to: {quesT.Goals[i].CurrentAmount}");
            quesT.Goals[i].Evaluate();
        }

        return quesT;
    }
   private void Start()
   {
        if(CurrentQuests.Count == 0)
        {

        }
        else
        {
            foreach (var quest in CurrentQuests)
            {
                quest.Initialize();
                quest.QuestCompleted.AddListener(OnQuestCompleted);

                GameObject questObj = Instantiate(questPrefab, questsContent);
                //questObj.transform.Find("Icon").GetComponent<Image>().sprite = quest.Information.Icon;

                questObj.GetComponent<Button>().onClick.AddListener(delegate
                {
                    questHolder.GetComponent<QuestWindow>().Initialize(quest);
                    questHolder.GetComponent<QuestWindow>().questOBJ = questObj;
                    questHolder.SetActive(true);
                });
            }
        }
        modifier();
   }

   public void Build(string buildingName)
   {
      EventManager.Instance.QueueEvent(new BuildingGameEvent(buildingName));
   }

   private void OnQuestCompleted(Quest quest)
   {
      questsContent.GetChild(CurrentQuests.IndexOf(quest)).Find("Checkmark").gameObject.SetActive(true);
        GameManager.current.GetCoins(quest.Reward.Currency);
        GameManager.current.GetXP(quest.Reward.XP);
   }
    private void OnApplicationQuit()
    {
        foreach(Quest quest in CurrentQuests)
        {
            QuestSave questSave = new QuestSave();

            questSave.ID = quest.Information.QuestID;
            questSave.questsID = quest.Information.QuestID;
            foreach (Quest.QuestGoal goal in quest.Goals)
            {
                questSave.progress.Add(goal.CurrentAmount);
            }

            GameManager.current.saveData.AddData(questSave);
        }
    }
}
