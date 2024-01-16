using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantingGoal : Quest.QuestGoal
{
    public string Plant;

    public override string GetDescription()
    {
        return $"Plant a {Plant}";
    }

    public override void Initialize()
    {
        base.Initialize();
        EventManager.Instance.AddListener<PlantingGameEvent>(OnPlanting);
    }

    private void OnPlanting(PlantingGameEvent eventInfo)
    {
        if (eventInfo.PlantingName == Plant)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
