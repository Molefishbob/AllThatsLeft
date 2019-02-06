using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VolumeControl : MonoBehaviour
{
    protected enum SoundType
    {
        SoundEffect,
        Music,
        Interface
    }

    protected AudioSource _audioSource;
    private float _fullVolume;
    protected SoundType _soundType;

    /// <summary>
    /// Is the audio source playing right now (Read Only)?
    /// </summary>
    public bool IsPlaying { get { return _audioSource.isPlaying; } }

    protected abstract void SetSoundType();

    protected virtual void Awake()
    {
        SetSoundType();
    }

    protected virtual void Start()
    {
        _fullVolume = _audioSource.volume;
        switch (_soundType)
        {
            case SoundType.SoundEffect:
                SoundManager.Instance.SoundsSFX.Add(this);
                _audioSource.volume = SoundManager.Instance.SFXVolume * SoundManager.Instance.MasterVolume * _fullVolume;
                _audioSource.mute = SoundManager.Instance.SFXMute || SoundManager.Instance.MasterMute;
                break;
            case SoundType.Music:
                SoundManager.Instance.SoundsMusic.Add(this);
                _audioSource.volume = SoundManager.Instance.MusicVolume * SoundManager.Instance.MasterVolume * _fullVolume;
                _audioSource.mute = SoundManager.Instance.MusicMute || SoundManager.Instance.MasterMute;
                break;
            case SoundType.Interface:
                SoundManager.Instance.SoundsUI.Add(this);
                _audioSource.volume = SoundManager.Instance.UIVolume * SoundManager.Instance.MasterVolume * _fullVolume;
                _audioSource.mute = SoundManager.Instance.UIMute || SoundManager.Instance.MasterMute;
                break;
            default:
                Debug.LogError("INVALID SOUND TYPE!!!!!");
                break;
        }
    }

    protected virtual void OnDestroy()
    {
        if (SoundManager.Instance != null)
        {
            switch (_soundType)
            {
                case SoundType.SoundEffect:
                    SoundManager.Instance.SoundsSFX.Remove(this);
                    break;
                case SoundType.Music:
                    SoundManager.Instance.SoundsMusic.Remove(this);
                    break;
                case SoundType.Interface:
                    SoundManager.Instance.SoundsUI.Remove(this);
                    break;
                default:
                    Debug.LogError("INVALID SOUND TYPE!!!!!");
                    break;
            }
        }
    }

    /// <summary>
    /// Sets audio source volume. Used only through Sound Manager!
    /// </summary>
    /// <param name="volume">The volume</param>
    public void SetVolume(float volume)
    {
        _audioSource.volume = volume * _fullVolume;
    }

    /// <summary>
    /// Mutes the audio source. Used only through Sound Manager!
    /// </summary>
    /// <param name="muted">Mute or not</param>
    public void Mute(bool muted)
    {
        _audioSource.mute = muted;
    }

    /// <summary>
    /// Stop the audio source from playing.
    /// </summary>
    public void StopSound()
    {
        _audioSource.Stop();
    }
}