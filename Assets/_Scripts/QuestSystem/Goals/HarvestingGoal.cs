using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingGoal : Quest.QuestGoal
{
    public string Plant;

    public override string GetDescription()
    {
        return $"Harvest a {Plant}";
    }

    public override void Initialize()
    {
        base.Initialize();
        EventManager.Instance.AddListener<HarvestingGameEvent>(OnHarvesting);
    }

    private void OnHarvesting(HarvestingGameEvent eventInfo)
    {
        if (eventInfo.HarvestingName == Plant)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
