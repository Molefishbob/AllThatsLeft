using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    // (Optional) Prevent non-singleton constructor use.
    protected AudioManager() { }

    public HashSet<VolumeControl> SoundsSFX { get; private set; } = new HashSet<VolumeControl>();
    public HashSet<VolumeControl> SoundsMusic { get; private set; } = new HashSet<VolumeControl>();
    public HashSet<VolumeControl> SoundsUI { get; private set; } = new HashSet<VolumeControl>();

    private float _volumeSFX;
    private float _volumeMusic;
    private float _volumeUI;
    private float _volumeMaster;
    private bool _muteSFX;
    private bool _muteMusic;
    private bool _muteUI;
    private bool _muteMaster;

    private void Awake()
    {
        _volumeSFX = PrefsManager.Instance.AudioVolumeSFX;
        _volumeMusic = PrefsManager.Instance.AudioVolumeMusic;
        _volumeUI = PrefsManager.Instance.AudioVolumeUI;
        _volumeMaster = PrefsManager.Instance.AudioVolumeMaster;
        _muteSFX = PrefsManager.Instance.AudioMuteSFX;
        _muteMusic = PrefsManager.Instance.AudioMuteMusic;
        _muteUI = PrefsManager.Instance.AudioMuteUI;
        _muteMaster = PrefsManager.Instance.AudioMuteMaster;

        SetVolumes(SoundsSFX, _volumeSFX);
        SetVolumes(SoundsMusic, _volumeMusic);
        SetVolumes(SoundsUI, _volumeUI);
        SetMutes(SoundsSFX, _muteSFX);
        SetMutes(SoundsMusic, _muteMusic);
        SetMutes(SoundsUI, _muteUI);
    }

    private void SetVolumes(HashSet<VolumeControl> set, float vol)
    {
        foreach (VolumeControl control in set)
        {
            if (control != null)
            {
                control.SetVolume(vol * _volumeMaster);
            }
        }
    }

    private void SetMutes(HashSet<VolumeControl> set, bool mute)
    {
        foreach (VolumeControl control in set)
        {
            if (control != null)
            {
                control.Mute(mute || _muteMaster);
            }
        }
    }

    /// <summary>
    /// Sound effect volume.
    /// </summary>
    public float SFXVolume
    {
        get
        {
            return _volumeSFX;
        }
        set
        {
            _volumeSFX = Mathf.Clamp(value, 0.0f, 1.0f);
            SetVolumes(SoundsSFX, _volumeSFX);
            PrefsManager.Instance.AudioVolumeSFX = _volumeSFX;
        }
    }

    /// <summary>
    /// Music volume.
    /// </summary>
    public float MusicVolume
    {
        get
        {
            return _volumeMusic;
        }
        set
        {
            _volumeMusic = Mathf.Clamp(value, 0.0f, 1.0f);
            SetVolumes(SoundsMusic, _volumeMusic);
            PrefsManager.Instance.AudioVolumeMusic = _volumeMusic;
        }
    }

    /// <summary>
    /// User interface volume.
    /// </summary>
    public float UIVolume
    {
        get
        {
            return _volumeUI;
        }
        set
        {
            _volumeUI = Mathf.Clamp(value, 0.0f, 1.0f);
            SetVolumes(SoundsUI, _volumeUI);
            PrefsManager.Instance.AudioVolumeUI = _volumeUI;
        }
    }

    /// <summary>
    /// Master volume.
    /// </summary>
    public float MasterVolume
    {
        get
        {
            return _volumeMaster;
        }
        private set
        {
            _volumeMaster = Mathf.Clamp(value, 0.0f, 1.0f);
            SetVolumes(SoundsSFX, _volumeSFX);
            SetVolumes(SoundsMusic, _volumeMusic);
            SetVolumes(SoundsUI, _volumeUI);
            PrefsManager.Instance.AudioVolumeMaster = _volumeMaster;
        }
    }

    /// <summary>
    /// Mutes/unmutes sound effects.
    /// </summary>
    public bool SFXMute
    {
        get
        {
            return _muteSFX;
        }
        set
        {
            _muteSFX = value;
            SetMutes(SoundsSFX, _muteSFX);
            PrefsManager.Instance.AudioMuteSFX = _muteSFX;
        }
    }

    /// <summary>
    /// Mutes/unmutes music.
    /// </summary>
    public bool MusicMute
    {
        get
        {
            return _muteMusic;
        }
        set
        {
            _muteMusic = value;
            SetMutes(SoundsMusic, _muteMusic);
            PrefsManager.Instance.AudioMuteMusic = _muteMusic;
        }
    }

    /// <summary>
    /// Mutes/unmutes user interface sounds.
    /// </summary>
    public bool UIMute
    {
        get
        {
            return _muteUI;
        }
        set
        {
            _muteUI = value;
            SetMutes(SoundsUI, _muteUI);
            PrefsManager.Instance.AudioMuteUI = _muteUI;
        }
    }

    /// <summary>
    /// Mutes/unmutes all sounds.
    /// </summary>
    public bool MasterMute
    {
        get
        {
            return _muteMaster;
        }
        set
        {
            _muteMaster = value;
            SetMutes(SoundsSFX, _muteSFX);
            SetMutes(SoundsMusic, _muteMusic);
            SetMutes(SoundsUI, _muteUI);
            PrefsManager.Instance.AudioMuteMaster = _muteMaster;
        }
    }
}