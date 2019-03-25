using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject _canvas;
    [SerializeField]
    private GameObject _mainMenuPanel = null;
    [SerializeField]
    private GameObject _optionsPanel = null;
    [SerializeField]
    private GameObject _quitPanel = null;
    [SerializeField]
    private EventSystem _eventSystem = null;
    [SerializeField]
    private Button _continueButton = null;
    [SerializeField]
    private GameObject _newGame = null;
    [SerializeField]
    private GameObject _masterVolume = null;

    private void Start() {
        // TODO: Check for save data and set continuebutton according to it

        _continueButton.interactable = false;
        _eventSystem.SetSelectedGameObject(_newGame);
    }

    public void StartGame() {
        GameManager.Instance.StartNewGame();
    }

    public void EnableMainMenuPanel()
    {
        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _quitPanel.SetActive(false);
        _eventSystem.UpdateModules();
        _eventSystem.SetSelectedGameObject(_mainMenuPanel.GetComponentInChildren<Button>().gameObject);
    }

    public void EnableOptionsPanel()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
        _eventSystem.UpdateModules();
        _eventSystem.SetSelectedGameObject(_masterVolume);
    }

    public void EnableConfirmQuit(){
        _mainMenuPanel.SetActive(false);
        _quitPanel.SetActive(true);
        _eventSystem.UpdateModules();
       _eventSystem.SetSelectedGameObject(_quitPanel.GetComponentInChildren<Button>().gameObject);
    }

    public void Quit()
    {
        Debug.Log("Quitted");
        GameManager.Instance.QuitGame();
    }
}
