using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSFXSound : SingleUISound
{
    protected override void Start()
    {
        base.Start();

        GameManager.Instance.OnGamePauseChanged += Pause;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

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