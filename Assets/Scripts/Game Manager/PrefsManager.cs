using UnityEngine;

public class PrefsManager : Singleton<PrefsManager>
{
    private const string
        keyVolumeSFX = "SFX Volume",
        keyVolumeMusic = "Music Volume",
        keyVolumeUI = "UI Volume",
        keyVolumeMaster = "Master Volume",
        keyMuteSFX = "SFX Mute",
        keyMuteMusic = "Music Mute",
        keyMuteUI = "UI Mute",
        keyMuteMaster = "Master Mute",
        keyInvertCameraY = "Invert Camera Y",
        keyInvertCameraX = "Invert Camera X";

    public event ValueChangedInt OnAudioVolumeSFXChanged;
    public event ValueChangedInt OnAudioVolumeMusicChanged;
    public event ValueChangedInt OnAudioVolumeUIChanged;
    public event ValueChangedInt OnAudioVolumeMasterChanged;
    public event ValueChangedBool OnAudioMuteSFXChanged;
    public event ValueChangedBool OnAudioMuteMusicChanged;
    public event ValueChangedBool OnAudioMuteUIChanged;
    public event ValueChangedBool OnAudioMuteMasterChanged;
    public event ValueChangedBool OnInvertedCameraYChanged;
    public event ValueChangedBool OnInvertedCameraXChanged;

    public void Save()
    {
        PlayerPrefs.Save();
    }

    public int AudioVolumeSFX
    {
        get
        {
            return PlayerPrefs.GetInt(keyVolumeSFX, 100);
        }
        set
        {
            PlayerPrefs.SetFloat(keyVolumeSFX, value);
        }
    }

    public int AudioVolumeMusic
    {
        get
        {
            return PlayerPrefs.GetInt(keyVolumeMusic, 100);
        }
        set
        {
            PlayerPrefs.SetFloat(keyVolumeMusic, value);
        }
    }

    public int AudioVolumeUI
    {
        get
        {
            return PlayerPrefs.GetInt(keyVolumeUI, 100);
        }
        set
        {
            PlayerPrefs.SetFloat(keyVolumeUI, value);
        }
    }

    public int AudioVolumeMaster
    {
        get
        {
            return PlayerPrefs.GetInt(keyVolumeMaster, 100);
        }
        set
        {
            PlayerPrefs.SetFloat(keyVolumeMaster, value);
        }
    }

    public bool AudioMuteSFX
    {
        get
        {
            return PlayerPrefs.GetInt(keyMuteSFX, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(keyMuteSFX, value ? 1 : 0);
        }
    }

    public bool AudioMuteMusic
    {
        get
        {
            return PlayerPrefs.GetInt(keyMuteMusic, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(keyMuteMusic, value ? 1 : 0);
        }
    }

    public bool AudioMuteUI
    {
        get
        {
            return PlayerPrefs.GetInt(keyMuteUI, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(keyMuteUI, value ? 1 : 0);
        }
    }

    public bool AudioMuteMaster
    {
        get
        {
            return PlayerPrefs.GetInt(keyMuteMaster, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(keyMuteMaster, value ? 1 : 0);
        }
    }

    public bool InvertedCameraY
    {
        get
        {
            return PlayerPrefs.GetInt(keyInvertCameraY, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(keyInvertCameraY, value ? 1 : 0);
            if (OnInvertedCameraYChanged != null)
            {
                OnInvertedCameraYChanged(value);
            }
        }
    }

    public bool InvertedCameraX
    {
        get
        {
            return PlayerPrefs.GetInt(keyInvertCameraX, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(keyInvertCameraX, value ? 1 : 0);
            if (OnInvertedCameraXChanged != null)
            {
                OnInvertedCameraXChanged(value);
            }
        }
    }
}