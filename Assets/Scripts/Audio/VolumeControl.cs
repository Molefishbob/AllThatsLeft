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
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        _fullVolume = _audioSource.volume;
        switch (_soundType)
        {
            case SoundType.SoundEffect:
                AudioManager.Instance.SoundsSFX.Add(this);
                _audioSource.volume = AudioManager.Instance.SFXVolume * AudioManager.Instance.MasterVolume * _fullVolume;
                _audioSource.mute = AudioManager.Instance.SFXMute || AudioManager.Instance.MasterMute;
                break;
            case SoundType.Music:
                AudioManager.Instance.SoundsMusic.Add(this);
                _audioSource.volume = AudioManager.Instance.MusicVolume * AudioManager.Instance.MasterVolume * _fullVolume;
                _audioSource.mute = AudioManager.Instance.MusicMute || AudioManager.Instance.MasterMute;
                break;
            case SoundType.Interface:
                AudioManager.Instance.SoundsUI.Add(this);
                _audioSource.volume = AudioManager.Instance.UIVolume * AudioManager.Instance.MasterVolume * _fullVolume;
                _audioSource.mute = AudioManager.Instance.UIMute || AudioManager.Instance.MasterMute;
                break;
            default:
                Debug.LogError("INVALID SOUND TYPE!!!!!");
                break;
        }
    }

    protected virtual void OnDestroy()
    {
        if (AudioManager.Instance != null)
        {
            switch (_soundType)
            {
                case SoundType.SoundEffect:
                    AudioManager.Instance.SoundsSFX.Remove(this);
                    break;
                case SoundType.Music:
                    AudioManager.Instance.SoundsMusic.Remove(this);
                    break;
                case SoundType.Interface:
                    AudioManager.Instance.SoundsUI.Remove(this);
                    break;
                default:
                    Debug.LogError("INVALID SOUND TYPE!!!!!");
                    break;
            }
        }
    }

    /// <summary>
    /// Sets audio source volume. Used only through Audio Manager!
    /// </summary>
    /// <param name="volume">The volume</param>
    public void SetVolume(float volume)
    {
        _audioSource.volume = volume * _fullVolume;
    }

    /// <summary>
    /// Mutes the audio source. Used only through Audio Manager!
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