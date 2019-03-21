using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Button _resumeButton = null;
    [SerializeField]
    private Button _ToMenu = null;
    [SerializeField]
    private Button _ToDesktop = null;
    private int _maxBotAmount = 0, _currentBotAmount = 0;

    private void Start()
    {

    }

    public void Resume() {
        GameManager.Instance.UnPauseGame();
    }

    public void ToMenu() {
        GameManager.Instance.ChangeToMainMenu();
    }

    public void Settings() {
        /// TODO: OPEN SETTINGS
    }

    public void ToDesktop() {
        GameManager.Instance.QuitGame();
    }
}
