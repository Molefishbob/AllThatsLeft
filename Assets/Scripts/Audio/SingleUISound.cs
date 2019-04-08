using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUISound : VolumeControl
{
    [SerializeField]
    protected float _pitchVariance = 0.25f;
    protected float _basePitch;

    protected override void OnEnable()
    {
        base.OnEnable();

        PrefsManager.Instance.OnAudioVolumeSFXChanged += SetVolumeSelf;
        PrefsManager.Instance.OnAudioVolumeMasterChanged += SetVolumeMaster;
        PrefsManager.Instance.OnAudioMuteSFXChanged += MuteSelf;
        PrefsManager.Instance.OnAudioMuteMasterChanged += MuteMaster;
    }

    protected virtual void Start()
    {
        _basePitch = _audioSource.pitch;
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