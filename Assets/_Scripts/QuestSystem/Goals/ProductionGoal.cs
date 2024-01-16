using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionGoal : Quest.QuestGoal
{
    public string Product;

    public override string GetDescription()
    {
        return $"Producing a {Product}";
    }

    public override void Initialize()
    {
        base.Initialize();
        EventManager.Instance.AddListener<ProductionGameEvent>(OnProduction);
    }

    private void OnProduction(ProductionGameEvent eventInfo)
    {
        if (eventInfo.ProductionName == Product)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}
