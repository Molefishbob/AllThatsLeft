using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingTimer : ScaledTimer {
    public int TimesCompleted { get; private set; } = 0;
    public float TotalTimeElapsed { get { return TimesCompleted * Duration + TimeElapsed; } }
    protected override void CompletedTimer() {
        _timer -= Duration;
        TimesCompleted++;
    }
}