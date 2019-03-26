using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsOneShotTimer : Timer
{
    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;
        if (!IsRunning) return;

        _timer += Time.deltaTime;
        if (_timer >= Duration)
        {
            _timer = Duration;
            IsRunning = false;
            CompletedTimer();
        }
    }
}