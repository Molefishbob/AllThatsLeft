using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VolumeControl : MonoBehaviour
{
    protected AudioSource _audioSource;
    protected float _fullVolume;

    /// <summary>
    /// Is the audio source playing right now (Read Only)?
    /// </summary>
    public bool IsPlaying { get { return _audioSource.isPlaying; } }

    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        _fullVolume = _audioSource.volume;
    }

    protected void SetVolumeSelf(int volume)
    {
        _audioSource.volume = (float)(volume * PrefsManager.Instance.AudioVolumeMaster) * _fullVolume / 10000f;
    }

    protected void MuteSelf(bool muted)
    {
        _audioSource.mute = muted || PrefsManager.Instance.AudioMuteMaster;
    }

    protected abstract void SetVolumeMaster(int volume);
    protected abstract void MuteMaster(bool muted);

    /// <summary>
    /// Stop the audio source from playing.
    /// </summary>
    public void StopSound()
    {
        _audioSource.Stop();
    }
}