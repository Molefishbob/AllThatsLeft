using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // (Optional) Prevent non-singleton constructor use.
    protected SoundManager() { }

    public HashSet<VolumeControl> SoundsSFX { get; private set; } = new HashSet<VolumeControl>();
    public HashSet<VolumeControl> SoundsMusic { get; private set; } = new HashSet<VolumeControl>();
    public HashSet<VolumeControl> SoundsUI { get; private set; } = new HashSet<VolumeControl>();

    //TODO: read values from player preferences!!!
    private float _volumeSFX = 1.0f;
    private float _volumeMusic = 1.0f;
    private float _volumeUI = 1.0f;
    private float _volumeMaster = 1.0f;
    private bool _muteSFX = false;
    private bool _muteMusic = false;
    private bool _muteUI = false;
    private bool _muteMaster = false;

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
            foreach (VolumeControl control in SoundsSFX)
            {
                if (control != null)
                {
                    control.SetVolume(_volumeSFX * _volumeMaster);
                }
            }
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
            foreach (VolumeControl control in SoundsMusic)
            {
                if (control != null)
                {
                    control.SetVolume(_volumeMusic * _volumeMaster);
                }
            }
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
            foreach (VolumeControl control in SoundsUI)
            {
                if (control != null)
                {
                    control.SetVolume(_volumeUI * _volumeMaster);
                }
            }
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
            foreach (VolumeControl control in SoundsSFX)
            {
                if (control != null)
                {
                    control.SetVolume(_volumeSFX * _volumeMaster);
                }
            }
            foreach (VolumeControl control in SoundsMusic)
            {
                if (control != null)
                {
                    control.SetVolume(_volumeMusic * _volumeMaster);
                }
            }
            foreach (VolumeControl control in SoundsUI)
            {
                if (control != null)
                {
                    control.SetVolume(_volumeUI * _volumeMaster);
                }
            }
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
            foreach (VolumeControl control in SoundsSFX)
            {
                control.Mute(_muteSFX || _muteMaster);
            }
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
            foreach (VolumeControl control in SoundsMusic)
            {
                control.Mute(_muteMusic || _muteMaster);
            }
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
            foreach (VolumeControl control in SoundsUI)
            {
                control.Mute(_muteUI || _muteMaster);
            }
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
            foreach (VolumeControl control in SoundsSFX)
            {
                control.Mute(_muteSFX || _muteMaster);
            }
            foreach (VolumeControl control in SoundsMusic)
            {
                control.Mute(_muteMusic || _muteMaster);
            }
            foreach (VolumeControl control in SoundsUI)
            {
                control.Mute(_muteUI || _muteMaster);
            }
        }
    }
}