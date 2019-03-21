using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

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
