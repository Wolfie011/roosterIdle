using System;
using UnityEngine;

[Serializable]
public class DailyRewardsData : Data
{
    public DateTime pauzeTime;
    public DateTime startTime;
    public double secondsLeft;
    public DateTime finishTime;
    public int strick;
}
