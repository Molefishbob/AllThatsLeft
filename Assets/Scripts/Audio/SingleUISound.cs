using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUISound : VolumeControl
{
    [SerializeField]
    protected float _pitchVariance = 0.25f;
    protected float _basePitch;

    protected virtual void Start()
    {
        _fullVolume = _audioSource.volume;
        _basePitch = _audioSource.pitch;

        PrefsManager.Instance.OnAudioVolumeUIChanged += SetVolumeSelf;
        PrefsManager.Instance.OnAudioVolumeMasterChanged += SetVolumeMaster;
        PrefsManager.Instance.OnAudioMuteUIChanged += MuteSelf;
        PrefsManager.Instance.OnAudioMuteMasterChanged += MuteMaster;

        _audioSource.volume = PrefsManager.Instance.AudioVolumeUI * PrefsManager.Instance.AudioVolumeMaster * _fullVolume;
        _audioSource.mute = PrefsManager.Instance.AudioMuteUI || PrefsManager.Instance.AudioMuteMaster;
    }

    protected virtual void OnDestroy()
    {
        if (PrefsManager.Instance != null)
        {
            PrefsManager.Instance.OnAudioVolumeSFXChanged -= SetVolumeMaster;
            PrefsManager.Instance.OnAudioVolumeMasterChanged -= SetVolumeMaster;
            PrefsManager.Instance.OnAudioMuteSFXChanged -= MuteMaster;
            PrefsManager.Instance.OnAudioMuteMasterChanged -= MuteMaster;
        }
    }

    protected void RandomizePitch()
    {
        if (_pitchVariance != 0.0f)
        {
            _audioSource.pitch = _basePitch * (1 + Random.Range(-_pitchVariance, _pitchVariance));
        }
        else
        {
            _audioSource.pitch = _basePitch;
        }
    }

    /// <summary>
    /// Plays the interface sound with randomized pitch.
    /// </summary>
    public virtual void PlaySound()
    {
        PlaySound(true);
    }

    /// <summary>
    /// Plays the interface sound.
    /// </summary>
    /// <param name="usePitch">Randomize the pitch</param>
    public virtual void PlaySound(bool usePitch)
    {
        if (usePitch)
        {
            RandomizePitch();
        }
        else
        {
            _audioSource.pitch = _basePitch;
        }
        _audioSource.Play();
    }

    protected override void SetVolumeMaster(int volume)
    {
        _audioSource.volume = (float)(volume * PrefsManager.Instance.AudioVolumeUI) * _fullVolume / 10000f;
    }

    protected override void MuteMaster(bool muted)
    {
        _audioSource.mute = muted || PrefsManager.Instance.AudioMuteUI;
    }
}