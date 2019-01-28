using UnityEngine;

public abstract class VolumeControl {
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
                SoundManager.Instance._soundsSFX.Add(this);
                _audioSource.mute = SoundManager.Instance.MutedSFX || SoundManager.Instance.MutedAll;
                _audioSource.volume = SoundManager.Instance.VolumeSFX * SoundManager.Instance.VolumeAll * _fullVolume;
                break;
            case SoundType.Music:
                SoundManager.Instance._soundsMusic.Add(this);
                _audioSource.mute = SoundManager.Instance.MutedMusic || SoundManager.Instance.MutedAll;
                _audioSource.volume = SoundManager.Instance.VolumeMusic * SoundManager.Instance.VolumeAll * _fullVolume;
                break;
            case SoundType.Interface:
                SoundManager.Instance._soundsUI.Add(this);
                _audioSource.mute = SoundManager.Instance.MutedUI || SoundManager.Instance.MutedAll;
                _audioSource.volume = SoundManager.Instance.VolumeUI * SoundManager.Instance.VolumeAll * _fullVolume;
                break;
            default:
            Debug.LogError("INVALID SOUND TYPE!!!!!");
                break;
        }
    }
    public void SetVolume(float volume) {
        _audioSource.volume = volume * _fullVolume;
    }
    public void Mute(bool muted) {
        _audioSource.mute = muted;
    }
    protected virtual void OnDestroy() {
        switch(_soundType) {
            case SoundType.SoundEffect:
                SoundManager.Instance._soundsSFX.Remove(this);
                break;
            case SoundType.Music:
                SoundManager.Instance._soundsMusic.Remove(this);
                break;
            case SoundType.Interface:
                SoundManager.Instance._soundsUI.Remove(this);
                break;
            default:
            Debug.LogError("INVALID SOUND TYPE!!!!!");
                break;
        }
    }
}