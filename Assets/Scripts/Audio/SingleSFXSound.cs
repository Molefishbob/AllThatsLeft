using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSFXSound : SingleUISound
{
    protected override void Start()
    {
        _fullVolume = _audioSource.volume;
        _basePitch = _audioSource.pitch;

        PrefsManager.Instance.OnAudioVolumeSFXChanged += SetVolumeSelf;
        PrefsManager.Instance.OnAudioVolumeMasterChanged += SetVolumeMaster;
        PrefsManager.Instance.OnAudioMuteSFXChanged += MuteSelf;
        PrefsManager.Instance.OnAudioMuteMasterChanged += MuteMaster;

        _audioSource.volume = PrefsManager.Instance.AudioVolumeSFX * PrefsManager.Instance.AudioVolumeMaster * _fullVolume;
        _audioSource.mute = PrefsManager.Instance.AudioMuteSFX || PrefsManager.Instance.AudioMuteMaster;

        GameManager.Instance.OnGamePauseChanged += Pause;
    }

    protected override void OnDestroy()
    {
        if (PrefsManager.Instance != null)
        {
            PrefsManager.Instance.OnAudioVolumeSFXChanged -= SetVolumeMaster;
            PrefsManager.Instance.OnAudioVolumeMasterChanged -= SetVolumeMaster;
            PrefsManager.Instance.OnAudioMuteSFXChanged -= MuteMaster;
            PrefsManager.Instance.OnAudioMuteMasterChanged -= MuteMaster;
        }

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

    protected override void SetVolumeMaster(int volume)
    {
        _audioSource.volume = (float)(volume * PrefsManager.Instance.AudioVolumeSFX) * _fullVolume / 10000f;
    }

    protected override void MuteMaster(bool muted)
    {
        _audioSource.mute = muted || PrefsManager.Instance.AudioMuteSFX;
    }
}