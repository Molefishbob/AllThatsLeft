using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledRepeatingTimer : Timer
{
    /// <summary>
    /// How many times the timer has completed.
    /// </summary>
    public int TimesCompleted { get; private set; } = 0;

    /// <summary>
    /// How many seconds total the timer has run.
    /// </summary>
    public float TotalTimeElapsed { get { return TimesCompleted * Duration + TimeElapsed; } }

    private void Update()
    {
        if (!IsRunning) return;

        _timer += Time.unscaledDeltaTime;
        if (_timer >= Duration)
        {
            _timer -= Duration;
            TimesCompleted++;
            CompletedTimer(false);
        }
    }
}