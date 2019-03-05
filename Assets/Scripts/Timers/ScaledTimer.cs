using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScaledTimer : Timer, IPauseable
{
    private bool _paused;

    protected virtual void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    private void FixedUpdate()
    {
        if (IsRunning && !_paused)
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

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }
}