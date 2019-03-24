using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSFXSound : SingleUISound
{
    protected override void OnEnable()
    {
        base.OnEnable();

        GameManager.Instance.OnGamePauseChanged += Pause;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePauseChanged -= Pause;
        }
    }

    public virtual void Pause(bool paused)
    {
        if (paused)
        {
            _audioSource.Pause();
        }
        else
        {
            _audioSource.UnPause();
        }
    }
}