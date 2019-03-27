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

    public void Resume()
    {
        GameManager.Instance.UnPauseGame();
        gameObject.SetActive(false);
    }

    public void ToMenu()
    {
        GameManager.Instance.ChangeToMainMenu();
    }

    public void Settings()
    {
        _settings.SetActive(true);
        _pauseMenu.SetActive(false);
        _confirmQuit.SetActive(false);

        _eventSystem.SetSelectedGameObject(_masterVolumeSlider);
    }
    public void ToPauseMenu()
    {
        _pauseMenu.SetActive(true);
        _settings.SetActive(false);
        _confirmQuit.SetActive(false);

        _eventSystem.SetSelectedGameObject(_resumeButton);
    }

    public void ToDesktop() {
        _confirmQuit.SetActive(true);
        _eventSystem.SetSelectedGameObject(_noButton);
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
