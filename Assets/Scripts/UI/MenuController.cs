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
    [SerializeField]
    private LoopingMusic _menuMusicPrefab = null;
    [SerializeField]
    private Animator _titleAnim = null;
    [SerializeField]
    private SingleUISound _buttonSound = null;
    public enum Page
    {
        MainMenu,
        VolumeSettings,
        ControlSettings,
        ConfirmationQuit
    }
    private Page _currentPage;

    [SerializeField]
    private LoadingScreen _loadingScreen = null;
    private Animator _anim;
    private UnscaledOneShotTimer _timer;

    private void Awake()
    {
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
        if (GameManager.Instance.LoadingScreen == null)
        {
            GameManager.Instance.LoadingScreen = Instantiate(_loadingScreen);
            DontDestroyOnLoad(GameManager.Instance.LoadingScreen);
        }

        if (GameManager.Instance.MenuMusic == null)
        {
            GameManager.Instance.MenuMusic = Instantiate(_menuMusicPrefab);
            DontDestroyOnLoad(GameManager.Instance.MenuMusic);
            GameManager.Instance.MenuMusic.PlayMusic();
        }
    }

    private void Start()
    {

        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _quitPanel.SetActive(false);

        _titleAnim.SetTrigger("GameStart");

    }

    private bool ButtonsUsed()
    {
        if (Input.GetAxis("Horizontal") > 0f)
            return true;
        if (Input.GetAxis("Vertical") > 0f)
            return true;
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0))
            return true;

        return false;
    }

    private void PlayButtonClick()
    {
        _buttonSound.PlaySound();
    }

    private void Update()
    {
        if (_eventSystem.IsPointerOverGameObject() && GameManager.Instance.ShowCursor && _eventSystem.currentSelectedGameObject != null)
            _eventSystem.SetSelectedGameObject(null);

        if (ButtonsUsed() && _eventSystem.currentSelectedGameObject == null)
        {
            switch (_currentPage)
            {
                case Page.MainMenu:
                    if (!_continueButton.interactable)
                        _eventSystem.SetSelectedGameObject(_newGame);
                    else
                        _eventSystem.SetSelectedGameObject(_continueButton.gameObject);
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

    private void OnDisable()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= ContinueAction;
            _timer.OnTimerCompleted -= StartGameAction;
            _timer.OnTimerCompleted -= QuitAction;
        }
    }

    private void ContinueAction()
    {
        GameManager.Instance.ContinueGame();
        _timer.OnTimerCompleted -= ContinueAction;
    }

    /// <summary>
    /// Called by the continue button to continue a previously saved game
    /// </summary>
    public void Continue()
    {
        PlayButtonClick();
        _timer.StartTimer(_buttonSound.Duration);
        _timer.OnTimerCompleted += ContinueAction;
    }

    private void StartGameAction()
    {
        GameManager.Instance.StartNewGame();
        _timer.OnTimerCompleted -= StartGameAction;
    }

    /// <summary>
    /// Called by the new game button to start a new game
    /// </summary>
    public void StartGame()
    {
        PlayButtonClick();
        _timer.StartTimer(_buttonSound.Duration);
        _timer.OnTimerCompleted += StartGameAction;
    }

    /// <summary>
    /// Switches the current panel to mainmenu panel
    /// 
    /// Enables the mainmenu panel and disables the other panels
    /// Sets the currentpage to be mainmenu
    /// Selects a button for the console peasants
    /// </summary>
    public void EnableMainMenuPanel()
    {
        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _quitPanel.SetActive(false);
        _continueButton.interactable = PrefsManager.Instance.SavedGameExists;
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

    /// <summary>
    /// Switches the current panel to mainmenu panel
    /// 
    /// Enables the mainmenu panel and disables the other panels
    /// Sets the currentpage to be mainmenu
    /// Selects a button for the console peasants
    /// </summary>
    /// <param name="playSound">Plays a button click sound if true</param>
    public void EnableMainMenuPanel(bool playSound)
    {
        if (playSound)
            _buttonSound.PlaySound();

        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _quitPanel.SetActive(false);
        _continueButton.interactable = PrefsManager.Instance.SavedGameExists;
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

    /// <summary>
    /// Changes the currentPage to VolumeSettings
    /// </summary>
    public void VolumeSettings()
    {
        PlayButtonClick();
        _currentPage = Page.VolumeSettings;
    }

    /// <summary>
    /// Changes the currentPage to ControlSettings
    /// </summary>
    public void ControlSettings()
    {
        PlayButtonClick();
        _currentPage = Page.ControlSettings;
    }

    /// <summary>
    /// Switches to the options panel
    /// 
    /// Enables the options panel which defaults to volumesettings
    /// Changes the currentPage to VolumeSettings
    /// Selects the master slider for the console peasants
    /// </summary>
    public void EnableOptionsPanel()
    {
        PlayButtonClick();
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
        _currentPage = Page.VolumeSettings;
        _eventSystem.UpdateModules();
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(_masterVolume);
    }

    /// <summary>
    /// Switches to the confirmquit panel when quit is pressed
    /// 
    /// Enables the confirmquit panel
    /// Changes the currentPage to ConfirmQuit
    /// Selects a button for the console peasants
    /// </summary>
    public void EnableConfirmQuit()
    {
        PlayButtonClick();
        _mainMenuPanel.SetActive(false);
        _quitPanel.SetActive(true);
        _currentPage = Page.ConfirmationQuit;
        _eventSystem.UpdateModules();
        _eventSystem.SetSelectedGameObject(_quitPanel.GetComponentInChildren<Button>().gameObject);
    }
    private void QuitAction()
    {
        GameManager.Instance.QuitGame();
        _timer.OnTimerCompleted -= QuitAction;
    }

    /// <summary>
    /// Tells the GameManager to quit the game
    /// </summary>
    public void Quit()
    {
        PlayButtonClick();
        _timer.StartTimer(_buttonSound.Duration);
        _timer.OnTimerCompleted += QuitAction;
    }
}
