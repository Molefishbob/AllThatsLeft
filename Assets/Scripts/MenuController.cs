using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject _canvas;
    [SerializeField]
    private GameObject _mainMenuPanel;
    [SerializeField]
    private GameObject _optionsPanel;
    [SerializeField]
    private GameObject _quitPanel;

    public void StartGame() {
        GameManager.Instance.StartNewGame();
    }

    public void EnableMainMenuPanel()
    {
        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _quitPanel.SetActive(false);
    }

    public void EnableOptionsPanel()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
    }

    public void EnableConfirmQuit(){
        _mainMenuPanel.SetActive(false);
        _quitPanel.SetActive(true);
    }

    public void Quit()
    {
        Debug.Log("Quitted");
        GameManager.Instance.QuitGame();
    }
}
