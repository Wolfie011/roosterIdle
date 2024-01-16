using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchingGoal : Quest.QuestGoal
{
    public string Hatch;

    public override string GetDescription()
    {
        return $"Build a {Hatch}";
    }

    public override void Initialize()
    {
        base.Initialize();
        EventManager.Instance.AddListener<HatchingGameEvent>(OnHatching);
    }

    private void OnHatching(HatchingGameEvent eventInfo)
    {
        if (eventInfo.HatchingName == Hatch)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
