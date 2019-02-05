using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingTimer : ScaledTimer {
    /// <summary>
    /// How many times the timer has completed.
    /// </summary>
    public int TimesCompleted { get; private set; } = 0;
    /// <summary>
    /// How many seconds total the timer has run.
    /// </summary>
    public float TotalTimeElapsed { get { return TimesCompleted * Duration + TimeElapsed; } }
    protected override void CompletedTimer() {
        _timer -= Duration;
        TimesCompleted++;
    }
}