using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledOneShotTimer : Timer
{
    private void Update()
    {
        if (!IsRunning) return;

        _timer += Time.unscaledDeltaTime;
        if (_timer >= Duration)
        {
            _timer = Duration;
            IsRunning = false;
            CompletedTimer(true);
        }
    }
}