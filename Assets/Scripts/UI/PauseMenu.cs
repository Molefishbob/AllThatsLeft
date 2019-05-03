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
    [SerializeField]
    private SingleUISound _buttonClickSound = null;
    private UnscaledOneShotTimer _timer;
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
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
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

    public void ButtonClickSound()
    {
        _buttonClickSound.PlaySound();
    }

    private void Update()
    {
        if (GameManager.Instance.ShowCursor && _eventSystem.IsPointerOverGameObject() && _eventSystem.currentSelectedGameObject != null)
        {
            _eventSystem.SetSelectedGameObject(null);
        }
        if (!GameManager.Instance.ShowCursor && ButtonsUsed() && _eventSystem.currentSelectedGameObject == null)
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

    private void OnDisable()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= ResumeGame;
            _timer.OnTimerCompleted -= Menu;
            _timer.OnTimerCompleted -= Quit;
        }
    }

    private void ResumeGame()
    {
        GameManager.Instance.UnPauseGame();
        gameObject.SetActive(false);
        _timer.OnTimerCompleted -= ResumeGame;
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void Resume()
    {
        _timer.StartTimer(_buttonClickSound.Duration);
        _timer.OnTimerCompleted += ResumeGame;
    }

    private void Menu()
    {
        gameObject.SetActive(false);
        GameManager.Instance.ChangeToMainMenu();
        _timer.OnTimerCompleted -= Menu;
    }

    /// <summary>
    /// Returns the game to mainmenu
    /// </summary>
    public void ToMenu()
    {
        _timer.StartTimer(_buttonClickSound.Duration);
        _timer.OnTimerCompleted += Menu;
    }

    /// <summary>
    /// Opens the settings panel
    /// </summary>
    public void Settings()
    {
        _timer.StopTimer();
        _currentPage = Page.VolumeSettings;
        _settings.SetActive(true);
        _pauseMenu.SetActive(false);
        if (_confirmQuit != null)
            _confirmQuit.SetActive(false);

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_masterVolumeSlider);
    }
    /// <summary>
    /// Opens the settings panel
    /// </summary>
    public void PageToVolumeSettings()
    {
        ButtonClickSound();
        _currentPage = Page.VolumeSettings;
    }
    /// <summary>
    /// Opens the settings panel
    /// </summary>
    public void PageToControlSettings()
    {
        ButtonClickSound();
        _currentPage = Page.ControlSettings;
    }

    /// <summary>
    /// Opens the mainscreen of pausemenu
    /// </summary>
    public void ToPauseMenu()
    {
        _timer.StopTimer();
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
        _timer.StopTimer();
        _currentPage = Page.ConfirmationQuit;
        _confirmQuit.SetActive(true);
        if (_settings != null) _settings.SetActive(false);
        _pauseMenu.SetActive(false);

        _eventSystem.SetSelectedGameObject(_noButton);
    }

    private void Quit()
    {
        _timer.OnTimerCompleted -= Quit;
        GameManager.Instance.QuitGame();
    }

    /// <summary>
    /// Tells the GameManager to quit the game
    /// </summary>
    public void QuitGame()
    {
        _timer.StartTimer(_buttonClickSound.Duration);
        _timer.OnTimerCompleted += Quit;
    }
}
