using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingMusic : VolumeControl
{
    protected override void OnEnable()
    {
        base.OnEnable();

        PrefsManager.Instance.OnAudioVolumeMusicChanged += SetVolumeSelf;
        PrefsManager.Instance.OnAudioVolumeMasterChanged += SetVolumeMaster;
        PrefsManager.Instance.OnAudioMuteMusicChanged += MuteSelf;
        PrefsManager.Instance.OnAudioMuteMasterChanged += MuteMaster;
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
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
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