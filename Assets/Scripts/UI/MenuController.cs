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
    [SerializeField]
    private GameObject _CamxSlider = null;
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
        bool temp = PrefsManager.Instance.SavedGameExists;
        
        _continueButton.interactable = temp;
        EnableMainMenuPanel();
    }

    private void Update() 
    {
        if (_eventSystem.IsPointerOverGameObject() && _eventSystem.currentSelectedGameObject != null) 
        {
            _eventSystem.SetSelectedGameObject(null);
        }
        if (Input.anyKeyDown && _eventSystem.currentSelectedGameObject == null) 
        {
            switch (_currentPage) {
                case Page.MainMenu:
                    _eventSystem.SetSelectedGameObject(_newGame);
                    break;
                case Page.VolumeSettings:
                    _eventSystem.SetSelectedGameObject(_masterVolume);
                    break;
                case Page.ControlSettings:
                    _eventSystem.SetSelectedGameObject(_CamxSlider);
                    break;
                case Page.ConfirmationQuit:
                    _eventSystem.SetSelectedGameObject(_quitPanel.GetComponentInChildren<Button>().gameObject);
                    break;
            }
        }
    }

    public void Continue()
    {
        GameManager.Instance.ContinueGame();
    }

    public void StartGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void EnableMainMenuPanel()
    {
        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _quitPanel.SetActive(false);
        _currentPage = Page.MainMenu;
        _eventSystem.UpdateModules();
        if (_continueButton.interactable) 
        {
            _eventSystem.SetSelectedGameObject(_continueButton.gameObject);
        } 
        else 
        {
            _eventSystem.SetSelectedGameObject(_newGame);
        }
    }

    public void VolumeSettings() 
    {
        _currentPage = Page.VolumeSettings;
    }

    public void ControlSettings() 
    {
        _currentPage = Page.ControlSettings;
    }

    public void EnableOptionsPanel()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
        _currentPage = Page.VolumeSettings;
        _eventSystem.UpdateModules();
        _eventSystem.SetSelectedGameObject(_masterVolume);
    }

    public void EnableConfirmQuit(){
        _mainMenuPanel.SetActive(false);
        _quitPanel.SetActive(true);
        _currentPage = Page.ConfirmationQuit;
        _eventSystem.UpdateModules();
       _eventSystem.SetSelectedGameObject(_quitPanel.GetComponentInChildren<Button>().gameObject);
    }

    public void Quit()
    {
        print("Quitted");
        GameManager.Instance.QuitGame();
    }
}
