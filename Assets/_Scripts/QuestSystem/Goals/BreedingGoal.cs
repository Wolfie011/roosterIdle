using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadingGoal : Quest.QuestGoal
{
    public string Breed;

    public override string GetDescription()
    {
        return $"Build a {Breed}";
    }

    public override void Initialize()
    {
        base.Initialize();
        EventManager.Instance.AddListener<BreedingGameEvent>(OnBreeding);
    }

    private void OnBreeding(BreedingGameEvent eventInfo)
    {
        if (eventInfo.BreedingName == Breed)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
