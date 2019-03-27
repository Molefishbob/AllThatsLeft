using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUISound : VolumeControl
{
    [SerializeField]
    protected float _pitchVariance = 0.25f;
    protected float _basePitch;
    protected bool _started = false;

    protected virtual void OnEnable()
    {
        PrefsManager.Instance.OnAudioVolumeSFXChanged += SetVolumeSelf;
        PrefsManager.Instance.OnAudioVolumeMasterChanged += SetVolumeMaster;
        PrefsManager.Instance.OnAudioMuteSFXChanged += MuteSelf;
        PrefsManager.Instance.OnAudioMuteMasterChanged += MuteMaster;

        if (_started)
        {
            _audioSource.volume = PrefsManager.Instance.AudioVolumeSFX * PrefsManager.Instance.AudioVolumeMaster * _fullVolume;
            _audioSource.mute = PrefsManager.Instance.AudioMuteSFX || PrefsManager.Instance.AudioMuteMaster;
        }
    }

    protected override void Start()
    {
        base.Start();
        _basePitch = _audioSource.pitch;
        _audioSource.volume = PrefsManager.Instance.AudioVolumeSFX * PrefsManager.Instance.AudioVolumeMaster * _fullVolume;
        _audioSource.mute = PrefsManager.Instance.AudioMuteSFX || PrefsManager.Instance.AudioMuteMaster;
        _started = true;
    }

    protected virtual void OnDisable()
    {
        if (PrefsManager.Instance != null)
        {
            PrefsManager.Instance.OnAudioVolumeSFXChanged -= SetVolumeSelf;
            PrefsManager.Instance.OnAudioVolumeMasterChanged -= SetVolumeMaster;
            PrefsManager.Instance.OnAudioMuteSFXChanged -= MuteSelf;
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
        _audioSource.volume = (float)(volume * PrefsManager.Instance.AudioVolumeSFX) * _fullVolume / 10000f;
    }

    protected override void MuteMaster(bool muted)
    {
        _audioSource.mute = muted || PrefsManager.Instance.AudioMuteSFX;
    }
}