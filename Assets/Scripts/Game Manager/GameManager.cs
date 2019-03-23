using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void ValueChangedInt(int amount);
public delegate void ValueChangedFloat(float amount);
public delegate void ValueChangedBool(bool value);

public enum MiniBotAbility
{
    Hack,
    Bomb,
    Tramp
}

public class GameManager : Singleton<GameManager>
{
    // (Optional) Prevent non-singleton constructor use.
    protected GameManager() { }
    public event ValueChangedInt OnBotAmountChanged;
    public event ValueChangedInt OnMaximumBotAmountChanged;
    public event ValueChangedBool OnGamePauseChanged;

    /// <summary>
    /// Is the game currently paused?
    /// </summary>
    public bool GamePaused { get; private set; }

    // TODO: make bot actions check this collection or remove ability system entirely
    public HashSet<MiniBotAbility> UsableMiniBotAbilities;

    public int CurrentBotAmount
    {
        get
        {
            return _currentBotAmount;
        }
        set
        {
            _currentBotAmount = value;

            if (OnBotAmountChanged != null)
            {
                OnBotAmountChanged(_currentBotAmount);
            }
        }
    }

    private int _currentBotAmount;

    public int MaximumBotAmount
    {
        get
        {
            return _maximumBotAmount;
        }
        set
        {
            _maximumBotAmount = value;

            if (OnMaximumBotAmountChanged != null)
            {
                OnMaximumBotAmountChanged(_maximumBotAmount);
            }
        }
    }

    private int _maximumBotAmount;

    /// <summary>
    /// Reference of the player.
    /// </summary>
    public PlayerMovement Player;

    private LevelManager _levelManager;

    /// <summary>
    /// Reference of the LevelManager.
    /// </summary>
    public LevelManager LevelManager
    {
        get
        {
            if (_levelManager == null)
            {
                Debug.LogError("LEVELMANAGER MISSING!!!!");
            }
            return _levelManager;
        }
        set
        {
            _levelManager = value;
        }
    }

    /// <summary>
    /// Reference of Bot Pool.
    /// </summary>
    public ControlledBotPool BotPool;

    /// <summary>
    /// Reference of Patrol Enemy Pool.
    /// </summary>
    public PatrolEnemyPool PatrolEnemyPool;

    /// <summary>
    /// Reference of Frog Enemy Pool.
    /// </summary>
    public FrogEnemyPool FrogEnemyPool;

    /// <summary>
    /// Reference of the camera.
    /// </summary>
    public ThirdPersonCamera Camera;

    /// <summary>
    /// ID of the current level.
    /// </summary>
    public int CurrentLevel { get; private set; } = 1;

    private float _timeScaleBeforePause = 1.0f;

    /// <summary>
    /// Pauses all in-game objects and sets timescale to 0.
    /// </summary>
    public void PauseGame()
    {
        if (!GamePaused)
        {
            GamePaused = true;
            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0;
            if (OnGamePauseChanged != null)
            {
                OnGamePauseChanged(true);
            }
        }
        else
        {
            Debug.LogWarning("Game is already paused.");
        }
    }

    /// <summary>
    /// Unpauses all in-game objects and sets timescale back to what it was before pausing.
    /// </summary>
    public void UnPauseGame()
    {
        if (GamePaused)
        {
            GamePaused = false;
            Time.timeScale = _timeScaleBeforePause;
            if (OnGamePauseChanged != null)
            {
                OnGamePauseChanged(false);
            }
        }
        else
        {
            Debug.LogWarning("Game is already unpaused.");
        }
    }

    public void ActivateGame(bool active)
    {
        Player?.gameObject.SetActive(active);
        Camera?.gameObject.SetActive(active);
        BotPool?.gameObject.SetActive(active);
        FrogEnemyPool?.gameObject.SetActive(active);
        PatrolEnemyPool?.gameObject.SetActive(active);
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
    }

    /// <summary>
    /// Quits the game to desktop.
    /// </summary>
    public void QuitGame()
    {
        PrefsManager.Instance.Save();
        Application.Quit();
    }

    /// <summary>
    /// Change scene to given build id.
    /// </summary>
    /// <param name="id">id of the scene in build settings</param>
    public void ChangeScene(int id)
    {
        if (id >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("No scene with ID: " + id);
            ChangeToMainMenu();
        }
        else
        {
            SceneManager.LoadScene(id);
        }
    }

    /// <summary>
    /// Changes scene to Main Menu (assuming it is the first scene in build settings).
    /// </summary>
    public void ChangeToMainMenu()
    {
        ActivateGame(false);
        ChangeScene(0);
    }

    /// <summary>
    /// Changes scene to next level.
    /// </summary>
    public void NextLevel()
    {
        CurrentLevel++;
        ChangeScene(CurrentLevel);
    }

    /// <summary>
    /// Loads the first level.
    /// </summary>
    public void StartNewGame()
    {
        CurrentLevel = 1;
        ChangeScene(CurrentLevel);
    }

    /// <summary>
    /// Continue game from a saved checkpoint.
    /// </summary>
    public void ContinueGame()
    {
        CurrentLevel = PrefsManager.Instance.Level;
        ChangeScene(CurrentLevel);
        LevelManager.SetCheckpoint(PrefsManager.Instance.CheckPoint);
        LevelManager.ResetLevel();
    }

    /// <summary>
    /// Reloads the active scene.
    /// </summary>
    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Undoes DontDestroyOnLoad for the given GameObject.
    /// </summary>
    /// <param name="go">GameObject in the special DontDestroyOnLoad scene.</param>
    public void UndoDontDestroy(GameObject go)
    {
        SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
    }
}