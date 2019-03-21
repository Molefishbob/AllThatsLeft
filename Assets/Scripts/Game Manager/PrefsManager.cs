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
        keyInvertCameraX = "Invert Camera X",
        keyCameraXSensitivity = "Camera X Sensitivity",
        keyCameraYSensitivity = "Camera Y Sensitivity";

    public event ValueChangedBool OnInvertedCameraYChanged;
    public event ValueChangedBool OnInvertedCameraXChanged;

    public event ValueChangedFloat OnCameraXSensitivityChanged;
    public event ValueChangedFloat OnCameraYSensitivityChanged;

    public void Save()
    {
        PlayerPrefs.Save();
    }

    public float AudioVolumeSFX
    {
        get
        {
            return PlayerPrefs.GetFloat(keyVolumeSFX, 1.0f);
        }
        set
        {
            PlayerPrefs.SetFloat(keyVolumeSFX, value);
        }
    }

    public float AudioVolumeMusic
    {
        get
        {
            return PlayerPrefs.GetFloat(keyVolumeMusic, 1.0f);
        }
        set
        {
            PlayerPrefs.SetFloat(keyVolumeMusic, value);
        }
    }

    public float AudioVolumeUI
    {
        get
        {
            return PlayerPrefs.GetFloat(keyVolumeUI, 1.0f);
        }
        set
        {
            PlayerPrefs.SetFloat(keyVolumeUI, value);
        }
    }

    public float AudioVolumeMaster
    {
        get
        {
            return PlayerPrefs.GetFloat(keyVolumeMaster, 1.0f);
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

    public float CameraXSensitivity
    {
        get
        {
            return PlayerPrefs.GetFloat(keyCameraXSensitivity, 2.0f);
        }
        set
        {
            PlayerPrefs.SetFloat(keyCameraXSensitivity, value);
            if (OnCameraXSensitivityChanged != null)
            {
                OnCameraXSensitivityChanged(value);
            }
        }
    }

    public float CameraYSensitivity
    {
        get
        {
            return PlayerPrefs.GetFloat(keyCameraYSensitivity, 2.0f);
        }
        set
        {
            PlayerPrefs.SetFloat(keyCameraYSensitivity, value);
            if (OnCameraYSensitivityChanged != null)
            {
                OnCameraYSensitivityChanged(value);
            }
        }
    }
}