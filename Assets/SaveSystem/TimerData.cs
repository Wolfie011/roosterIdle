using System;
using UnityEngine;

[Serializable]
public class TimerData : Data
{
    public string Name;
    public DateTime startTime;
    public DateTime pauzeTime;
    public TimeSpan timeToFinish;
    public DateTime finishTime;
    public double secondsLeft;
}
