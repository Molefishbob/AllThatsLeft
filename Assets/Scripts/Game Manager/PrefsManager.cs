using UnityEngine;

public class PrefsManager : Singleton<PrefsManager>
{
    private const string
        keyVolumeSFX = "SFX Volume",
        keyVolumeMusic = "Music Volume",
        keyVolumeMaster = "Master Volume",
        keyMuteSFX = "SFX Mute",
        keyMuteMusic = "Music Mute",
        keyMuteMaster = "Master Mute",
        keyLevel = "Level",
        keyCheckPoint = "Check Point",
        keyInvertCameraY = "Invert Camera Y",
        keyInvertCameraX = "Invert Camera X",
        keyCameraXSensitivity = "Camera X Sensitivity",
        keyCameraYSensitivity = "Camera Y Sensitivity";

    public event ValueChangedInt OnAudioVolumeSFXChanged;
    public event ValueChangedInt OnAudioVolumeMusicChanged;
    public event ValueChangedInt OnAudioVolumeMasterChanged;
    public event ValueChangedBool OnAudioMuteSFXChanged;
    public event ValueChangedBool OnAudioMuteMusicChanged;
    public event ValueChangedBool OnAudioMuteMasterChanged;

    public event ValueChangedBool OnInvertedCameraYChanged;
    public event ValueChangedBool OnInvertedCameraXChanged;

    public event ValueChangedFloat OnCameraXSensitivityChanged;
    public event ValueChangedFloat OnCameraYSensitivityChanged;

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
            PlayerPrefs.SetInt(keyVolumeSFX, value);
            if (OnAudioVolumeSFXChanged != null)
            {
                OnAudioVolumeSFXChanged(value);
            }
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
            PlayerPrefs.SetInt(keyVolumeMusic, value);
            if (OnAudioVolumeMusicChanged != null)
            {
                OnAudioVolumeMusicChanged(value);
            }
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
            PlayerPrefs.SetInt(keyVolumeMaster, value);
            if (OnAudioVolumeMasterChanged != null)
            {
                OnAudioVolumeMasterChanged(value);
            }
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
            if (OnAudioMuteSFXChanged != null)
            {
                OnAudioMuteSFXChanged(value);
            }
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
            if (OnAudioMuteMusicChanged != null)
            {
                OnAudioMuteMusicChanged(value);
            }
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
            if (OnAudioMuteMasterChanged != null)
            {
                OnAudioMuteMasterChanged(value);
            }
        }
    }

    public int Level
    {
        get
        {
            return PlayerPrefs.GetInt(keyLevel, 1);
        }
        set
        {
            PlayerPrefs.SetInt(keyVolumeMaster, value);
        }
    }

    public int CheckPoint
    {
        get
        {
            return PlayerPrefs.GetInt(keyCheckPoint, 0);
        }
        set
        {
            PlayerPrefs.SetInt(keyCheckPoint, value);
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