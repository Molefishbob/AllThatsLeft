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
    private LoadingScreen _loadingScreen = null;

    private void Awake()
    {
        if (GameManager.Instance.LoadingScreen == null)
        {
            GameManager.Instance.LoadingScreen = Instantiate(_loadingScreen);
            DontDestroyOnLoad(GameManager.Instance.LoadingScreen);
        }
    }

    private void Start() 
    {
        bool temp = PrefsManager.Instance.SavedGameExists;
        
        _continueButton.interactable = temp;
        if (temp) 
        {
            _eventSystem.SetSelectedGameObject(_continueButton.gameObject);
        } 
        else 
        {
            _eventSystem.SetSelectedGameObject(_newGame);
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
        print("Quitted");
        GameManager.Instance.QuitGame();
    }
}
