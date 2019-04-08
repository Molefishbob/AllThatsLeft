using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingMusic : VolumeControl
{
    protected bool _started = false;

    protected virtual void OnEnable()
    {
        PrefsManager.Instance.OnAudioVolumeMusicChanged += SetVolumeSelf;
        PrefsManager.Instance.OnAudioVolumeMasterChanged += SetVolumeMaster;
        PrefsManager.Instance.OnAudioMuteMusicChanged += MuteSelf;
        PrefsManager.Instance.OnAudioMuteMasterChanged += MuteMaster;

        if (_started)
        {
            _audioSource.volume = PrefsManager.Instance.AudioVolumeMusic * PrefsManager.Instance.AudioVolumeMaster * _fullVolume;
            _audioSource.mute = PrefsManager.Instance.AudioMuteMusic || PrefsManager.Instance.AudioMuteMaster;
        }
    }

    protected override void Start()
    {
        base.Start();
        _audioSource.volume = PrefsManager.Instance.AudioVolumeMusic * PrefsManager.Instance.AudioVolumeMaster * _fullVolume;
        _audioSource.mute = PrefsManager.Instance.AudioMuteMusic || PrefsManager.Instance.AudioMuteMaster;
        _started = true;
    }

    protected virtual void OnDisable()
    {
        if (PrefsManager.Instance != null)
        {
            PrefsManager.Instance.OnAudioVolumeMusicChanged -= SetVolumeSelf;
            PrefsManager.Instance.OnAudioVolumeMasterChanged -= SetVolumeMaster;
            PrefsManager.Instance.OnAudioMuteMusicChanged -= MuteSelf;
            PrefsManager.Instance.OnAudioMuteMasterChanged -= MuteMaster;
        }
    }

    /// <summary>
    /// Plays the music.
    /// </summary>
    public virtual void PlayMusic()
    {
        _audioSource.Play();
    }

    protected override void SetVolumeMaster(int volume)
    {
        _audioSource.volume = (float)(volume * PrefsManager.Instance.AudioVolumeMusic) * _fullVolume / 10000f;
    }

    protected override void MuteMaster(bool muted)
    {
        _audioSource.mute = muted || PrefsManager.Instance.AudioMuteMusic;
    }
}