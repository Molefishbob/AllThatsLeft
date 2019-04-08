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
    private GameObject _masterVolumeSlider = null, _noButton = null, _resumeButton = null;

    private void Start()
    {
        _eventSystem.SetSelectedGameObject(_resumeButton);
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
        GameManager.Instance.UnPauseGame();
        GameManager.Instance.ChangeToMainMenu();
    }

    /// <summary>
    /// Opens the settings panel
    /// </summary>
    public void Settings()
    {
        _settings.SetActive(true);
        _pauseMenu.SetActive(false);
        _confirmQuit.SetActive(false);

        _eventSystem.SetSelectedGameObject(_masterVolumeSlider);
    }

    /// <summary>
    /// Opens the mainscreen of pausemenu
    /// </summary>
    public void ToPauseMenu()
    {
        _pauseMenu.SetActive(true);
        _settings.SetActive(false);
        _confirmQuit.SetActive(false);

        _eventSystem.SetSelectedGameObject(_resumeButton);
    }

    /// <summary>
    /// Opens confirmquit screen
    /// </summary>
    public void ToDesktop() {
        _confirmQuit.SetActive(true);
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
