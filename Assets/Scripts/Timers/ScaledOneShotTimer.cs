using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledOneShotTimer : Timer
{
    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;
        if (!IsRunning) return;

        _timer += Time.deltaTime;
        if (_timer >= Duration)
        {
            _timer = Duration;
            IsRunning = false;
            CompletedTimer(true);
        }
    }
}