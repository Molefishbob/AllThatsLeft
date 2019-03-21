using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private const string PercentageFormat = " %";
    [SerializeField]
    private TMP_Text _sfxText = null, _musicText = null, _UISoundText = null, _masterText = null;
    [SerializeField]
    private Slider _sfxSlider = null, _musicSlider = null, _UISoundSlider = null, _masterSlider = null;
    [SerializeField]
    private Toggle _volumeSettings = null, _gameplaySettings = null, _controlSettings = null;
    private const bool True = true;
    private const bool False = false;

    // Start is called before the first frame update
    void Awake()
    {
        _sfxText.SetText(_sfxSlider.value + PercentageFormat);
        _musicText.SetText(_musicSlider.value + PercentageFormat);
        _UISoundText.SetText(_UISoundSlider.value + PercentageFormat);
        _masterText.SetText(_masterSlider.value + PercentageFormat);
        _volumeSettings.interactable = !True;
    }

    public void SFXPercentage() {
        _sfxText.SetText(_sfxSlider.value + PercentageFormat);
    }
    public void MusicPercentage() {
        _musicText.SetText(_musicSlider.value + PercentageFormat);
    }
    public void UISoundPercentage() {
        _UISoundText.SetText(_UISoundSlider.value + PercentageFormat);
    }
    public void MasterSoundPercentage() {
        _masterText.SetText(_masterSlider.value + PercentageFormat);
    }

    public void VolumeSettings() {
        _volumeSettings.interactable = !True;
        _controlSettings.interactable = !False;
        _gameplaySettings.interactable = !False;
    }
    public void GameplaySettings() {
        _gameplaySettings.interactable = !True;
        _controlSettings.interactable = !False;
        _volumeSettings.interactable = !False;
    }
    public void ControlSettings() {
        _controlSettings.interactable = !True;
        _volumeSettings.interactable = !False;
        _gameplaySettings.interactable = !False;
    }
}
