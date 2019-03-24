using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScaledTimer : Timer
{
    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }

        if (IsRunning)
        {
            _timer += Time.deltaTime;
            if (_timer >= Duration)
            {
                _timer = Duration;
                CompletedTimer();
                if (IsTargeted)
                {
                    _timedObject.TimedAction();
                }
            }
        }
    }
}