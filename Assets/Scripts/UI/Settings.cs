using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour
{
    private const string PercentageFormat = " %";
    [SerializeField]
    private EventSystem  _eventSystem = null;
    [SerializeField]
    private Button _backButton = null;
    [SerializeField]
    private TMP_Text  _musicText = null, _sFXText = null, _masterText = null;
    [SerializeField]
    private Slider _musicSlider = null, _sFXSlider = null, _masterSlider = null;
    [SerializeField]
    private Toggle _musicMute = null, _sFXMute = null, _masterMute = null;
    [SerializeField]
    private Toggle _volumeSettings = null, _controlSettings = null;
    [SerializeField]
    private GameObject _volume = null, _controls = null;
    [SerializeField]
    private Toggle _invertedXAxis = null, _invertedYAxis = null;
    [SerializeField]
    private TMP_Text _xSensText = null, _ySensText = null, _zoomSpeedText = null, _fovText = null;
    [SerializeField]
    private Slider _xSensSlider = null, _ySensSlider = null, _zoomSpeedSlider = null, _fovSlider = null;

    private const bool True = true;
    private const bool False = false;

    private void OnEnable() {
        VolumeSettings();
        _musicSlider.value = PrefsManager.Instance.AudioVolumeMusic;
        _sFXSlider.value = PrefsManager.Instance.AudioVolumeSFX;
        _masterSlider.value = PrefsManager.Instance.AudioVolumeMaster;

        _musicMute.isOn = PrefsManager.Instance.AudioMuteMusic;
        _masterMute.isOn = PrefsManager.Instance.AudioMuteMaster;
        _sFXMute.isOn = PrefsManager.Instance.AudioMuteSFX;

        _xSensSlider.value = PrefsManager.Instance.CameraXSensitivity;
        _ySensSlider.value = PrefsManager.Instance.CameraYSensitivity;
        _zoomSpeedSlider.value = PrefsManager.Instance.ZoomSpeed;
        _fovSlider.value = PrefsManager.Instance.FieldOfView;

        _xSensText.SetText(FormatToDecimalNumber(_xSensSlider.value));
        _ySensText.SetText(FormatToDecimalNumber(_ySensSlider.value));
        _zoomSpeedText.SetText(FormatToDecimalNumber(_zoomSpeedSlider.value));
        _fovText.SetText(_fovSlider.value.ToString());
        _invertedXAxis.isOn = PrefsManager.Instance.InvertedCameraX;
        _invertedYAxis.isOn = PrefsManager.Instance.InvertedCameraY;

        _musicText.SetText(_musicSlider.value + PercentageFormat);
        _sFXText.SetText(_sFXSlider.value + PercentageFormat);
        _masterText.SetText(_masterSlider.value + PercentageFormat);
    }

    private string FormatToDecimalNumber(float value) {
        return (value / 10f).ToString("N1");
    }

    public void MusicPercentage() {
        _musicText.SetText(_musicSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeMusic =  Mathf.RoundToInt(_musicSlider.value);
    }
    public void UISoundPercentage() {
        _sFXText.SetText(_sFXSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeSFX = Mathf.RoundToInt(_sFXSlider.value);
    }
    public void MasterSoundPercentage() {
        _masterText.SetText(_masterSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeMaster = Mathf.RoundToInt(_masterSlider.value);
    }
    public void MuteMusic()
    {
        PrefsManager.Instance.AudioMuteMusic = !PrefsManager.Instance.AudioMuteMusic;
    }
    public void MuteSound()
    {
        PrefsManager.Instance.AudioMuteSFX = !PrefsManager.Instance.AudioMuteSFX;
    }
    public void MuteMaster()
    {
        PrefsManager.Instance.AudioMuteMaster = !PrefsManager.Instance.AudioMuteMaster;
    }

    public void VolumeSettings() {
        _volumeSettings.interactable = !True;
        _controlSettings.interactable = !False;

        _volume.SetActive(!False);
        _controls.SetActive(!True);

        _eventSystem.SetSelectedGameObject(_masterSlider.gameObject);
        Navigation nav = _backButton.navigation;

        nav.selectOnUp = _controlSettings.GetComponent<Selectable>();
        nav.selectOnRight = _controlSettings.GetComponent<Selectable>();

        _backButton.navigation = nav;

    }
    public void ControlSettings() {
        _controlSettings.interactable = !True;
        _volumeSettings.interactable = !False;

        _volume.SetActive(!True);
        _controls.SetActive(!False);

        _eventSystem.SetSelectedGameObject(_xSensSlider.gameObject);

         Navigation nav = _backButton.navigation;

        nav.selectOnUp = _volumeSettings.GetComponent<Selectable>();
        nav.selectOnLeft = _volumeSettings.GetComponent<Selectable>();

        _backButton.navigation = nav;
    }
    public void InvertedXAxis()
    {
        PrefsManager.Instance.InvertedCameraX = _invertedXAxis.isOn;
        
    }
    public void InvertedYAxis()
    {
        PrefsManager.Instance.InvertedCameraY = _invertedYAxis.isOn;
    }
    public void CameraXSensitivity()
    {
        
        _xSensText.SetText(FormatToDecimalNumber(_xSensSlider.value));
        PrefsManager.Instance.CameraXSensitivity = Mathf.RoundToInt(_xSensSlider.value);
    }
    public void CameraYSensitivity()
    {
        _ySensText.SetText(FormatToDecimalNumber(_ySensSlider.value));
        PrefsManager.Instance.CameraYSensitivity = Mathf.RoundToInt(_ySensSlider.value);
    }
    public void CameraZoomSpeed()
    {
        _zoomSpeedText.SetText(FormatToDecimalNumber(_zoomSpeedSlider.value));
        PrefsManager.Instance.ZoomSpeed = Mathf.RoundToInt(_zoomSpeedSlider.value);
    }public void FieldOfView()
    {
        _fovText.SetText(_fovSlider.value.ToString());
        PrefsManager.Instance.FieldOfView = Mathf.RoundToInt(_fovSlider.value);
    }
}
