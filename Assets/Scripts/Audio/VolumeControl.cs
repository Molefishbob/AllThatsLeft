using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VolumeControl : MonoBehaviour {
    protected enum SoundType {
        SoundEffect,
        Music,
        Interface
    }
    protected AudioSource _audioSource;
    private float _fullVolume;
    protected SoundType _soundType;
    protected virtual void Awake() {
        SetSoundType();
    }
    protected virtual void Start() {
        _fullVolume = _audioSource.volume;
        switch(_soundType) {
            case SoundType.SoundEffect:
                SoundManager.Instance.SoundsSFX.Add(this);
                _audioSource.mute = SoundManager.Instance.MutedSFX || SoundManager.Instance.MutedAll;
                _audioSource.volume = SoundManager.Instance.VolumeSFX * SoundManager.Instance.VolumeAll * _fullVolume;
                break;
            case SoundType.Music:
                SoundManager.Instance.SoundsMusic.Add(this);
                _audioSource.mute = SoundManager.Instance.MutedMusic || SoundManager.Instance.MutedAll;
                _audioSource.volume = SoundManager.Instance.VolumeMusic * SoundManager.Instance.VolumeAll * _fullVolume;
                break;
            case SoundType.Interface:
                SoundManager.Instance.SoundsUI.Add(this);
                _audioSource.mute = SoundManager.Instance.MutedUI || SoundManager.Instance.MutedAll;
                _audioSource.volume = SoundManager.Instance.VolumeUI * SoundManager.Instance.VolumeAll * _fullVolume;
                break;
            default:
                Debug.LogError("INVALID SOUND TYPE!!!!!");
                break;
        }
    }
    protected virtual void OnDestroy() {
        if(SoundManager.Instance != null) {
            switch(_soundType) {
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
    public void SetVolume(float volume) {
        _audioSource.volume = volume * _fullVolume;
    }
    public void Mute(bool muted) {
        _audioSource.mute = muted;
    }
    public void StopSound() {
        _audioSource.Stop();
    }
    protected abstract void SetSoundType();
}