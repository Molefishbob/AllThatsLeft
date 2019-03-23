using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRepeatingTimer : Timer
{
    /// <summary>
    /// How many times the timer has completed.
    /// </summary>
    public int TimesCompleted { get; private set; } = 0;

    /// <summary>
    /// How many seconds total the timer has run.
    /// </summary>
    public float TotalTimeElapsed { get { return TimesCompleted * Duration + TimeElapsed; } }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;
        if (!IsRunning) return;

        _timer += Time.deltaTime;
        if (_timer >= Duration)
        {
            _timer -= Duration;
            TimesCompleted++;
            CompletedTimer();
        }
    }
}