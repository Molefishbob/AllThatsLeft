using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private EventSystem _eventSystem = null;
    [SerializeField]
    private GameObject _settings = null, _pauseMenu = null, _confirmQuit = null;
    [SerializeField]
    private GameObject _masterVolumeSlider = null, _noButton = null, _resumeButton = null, _camxSlider = null;
    public enum Page
    {
        MainMenu,
        VolumeSettings,
        ControlSettings,
        ConfirmationQuit
    }
    private Page _currentPage;

    private void Start()
    {
        _currentPage = Page.MainMenu;
        _eventSystem.SetSelectedGameObject(_resumeButton);
    }

    private bool ButtonsUsed()
    {
        if (Input.GetAxis("Horizontal") > 0f)
            return true;
        if (Input.GetAxis("Vertical") > 0f)
            return true;
        if (Input.anyKeyDown)
            return true;

        return false;
    }

    private void Update()
    {
        if (_eventSystem.IsPointerOverGameObject() && _eventSystem.currentSelectedGameObject != null)
        {
            _eventSystem.SetSelectedGameObject(null);
        }
        if (ButtonsUsed() && _eventSystem.currentSelectedGameObject == null)
        {
            switch (_currentPage)
            {
                case Page.MainMenu:
                        _eventSystem.SetSelectedGameObject(_resumeButton);
                    break;
                case Page.VolumeSettings:
                    _eventSystem.SetSelectedGameObject(_masterVolumeSlider);
                    break;
                case Page.ControlSettings:
                    _eventSystem.SetSelectedGameObject(_camxSlider);
                    break;
                case Page.ConfirmationQuit:
                    _eventSystem.SetSelectedGameObject(_noButton);
                    break;
            }
        }
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void Resume()
    {
        GameManager.Instance.UnPauseGame();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Returns the game to mainmenu
    /// </summary>
    public void ToMenu()
    {
        gameObject.SetActive(false);
        GameManager.Instance.ChangeToMainMenu();
    }

    /// <summary>
    /// Opens the settings panel
    /// </summary>
    public void Settings()
    {
        _currentPage = Page.VolumeSettings;
        _settings.SetActive(true);
        _pauseMenu.SetActive(false);
        if (_confirmQuit != null) _confirmQuit.SetActive(false);

        _eventSystem.SetSelectedGameObject(_masterVolumeSlider);
    }
    /// <summary>
    /// Opens the settings panel
    /// </summary>
    public void PageToVolumeSettings()
    {
        _currentPage = Page.VolumeSettings;
    }
    /// <summary>
    /// Opens the settings panel
    /// </summary>
    public void PageToControlSettings()
    {
        _currentPage = Page.ControlSettings;
    }

    /// <summary>
    /// Opens the mainscreen of pausemenu
    /// </summary>
    public void ToPauseMenu()
    {
        _currentPage = Page.MainMenu;
        _pauseMenu.SetActive(true);
        if (_settings != null) _settings.SetActive(false);
        if (_confirmQuit != null) _confirmQuit.SetActive(false);

        _eventSystem.SetSelectedGameObject(_resumeButton);
    }

    /// <summary>
    /// Opens confirmquit screen
    /// </summary>
    public void ToDesktop()
    {
        _currentPage = Page.ConfirmationQuit;
        _confirmQuit.SetActive(true);
        if (_settings != null) _settings.SetActive(false);
        _pauseMenu.SetActive(false);

        _eventSystem.SetSelectedGameObject(_noButton);
    }

    /// <summary>
    /// Tells the GameManager to quit the game
    /// </summary>
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
