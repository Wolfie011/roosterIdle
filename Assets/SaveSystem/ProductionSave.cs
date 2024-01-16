using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProductionSave : Data
{
    public List<string> productsQue = new List<string>();
    public DateTime pauzeTime;
    public double secondsLeft;
}
