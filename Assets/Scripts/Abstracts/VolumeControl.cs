using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VolumeControl : MonoBehaviour, IPauseable {
    private enum SoundType {
        SoundEffect,
        Music,
        Interface
    }
    protected AudioSource _audioSource;
    private float _fullVolume;
    [SerializeField]
    private SoundType _soundType = SoundType.SoundEffect;
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
        AddToPauseCollection();
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
    public void Pause() {
        _audioSource.Pause();
    }
    public void UnPause() {
        _audioSource.UnPause();
    }
    public void AddToPauseCollection() {
        GameManager.Instance.Pauseables.Add(this);
    }
    public void RemoveFromPauseCollection() {
        GameManager.Instance.Pauseables.Remove(this);
    }
    protected virtual void OnDestroy() {
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
        RemoveFromPauseCollection();
    }
}