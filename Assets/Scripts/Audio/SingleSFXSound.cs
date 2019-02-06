using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSFXSound : SingleUISound, IPauseable
{
    protected bool _paused;

    protected override void Start()
    {
        base.Start();
        _paused = GameManager.Instance.GamePaused;
        if (_paused)
        {
            _audioSource.Pause();
        }
        GameManager.Instance.AddPauseable(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    /// <summary>
    /// Plays the sound effect.
    /// </summary>
    /// <param name="usePitch">Randomize the pitch</param>
    public override void PlaySound(bool usePitch)
    {
        if (!_paused)
        {
            base.PlaySound(usePitch);
        }
    }

    protected override void SetSoundType()
    {
        _soundType = SoundType.SoundEffect;
    }

    public virtual void Pause()
    {
        _paused = true;
        _audioSource.Pause();
    }

    public virtual void UnPause()
    {
        _paused = false;
        _audioSource.UnPause();
    }
}