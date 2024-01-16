using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HatcherSave : Data
{
    public List<bool> slotStatus;
    public List<string> slotAssetName;
    public List<string> slotAssetID;
    public List<TimerData> timerDatas;
    public List<bool> isCountdown;
}
