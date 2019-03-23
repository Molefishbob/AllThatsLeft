using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GenericEvent();
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
    private HashSet<IPauseable> _pauseables = new HashSet<IPauseable>();

    /// <summary>
    /// Is the game currently paused?
    /// </summary>
    public bool GamePaused { get; private set; }

    // TODO: make bot actions check this collection
    public HashSet<MiniBotAbility> UsableMiniBotAbilities;

    public int CurrentBotAmount {
        get 
        { 
            return _currentBotAmount; 
        }
        set 
        {
            _currentBotAmount = value;

            if (OnBotAmountChanged != null) {
                OnBotAmountChanged(_currentBotAmount);
            }
        }
    }

    private int _currentBotAmount;

    public int MaximumBotAmount {
        get 
        { 
            return _maximumBotAmount; 
        }
        set 
        {
            _maximumBotAmount = value;

            if (OnMaximumBotAmountChanged != null) {
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

    public ControlledBotPool BotPool;

    public PatrolEnemyPool PatrolEnemyPool;
    public FrogEnemyPool FrogEnemyPool;

    public ThirdPersonCamera Camera;

    private float _timeScaleBeforePause = 1.0f;

    private void Awake()
    {
        AudioManager.Instance.Init();
    }

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
            foreach (IPauseable item in _pauseables)
            {
                if (item != null)
                {
                    item.Pause();
                }
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
            foreach (IPauseable item in _pauseables)
            {
                if (item != null)
                {
                    item.UnPause();
                }
            }
        }
        else
        {
            Debug.LogWarning("Game is already unpaused.");
        }
    }

    /// <summary>
    /// Add a pauseable item reference to be paused by PauseGame.
    /// </summary>
    /// <param name="pauseable">Class that implements IPauseable.</param>
    public void AddPauseable(IPauseable pauseable)
    {
        _pauseables.Add(pauseable);
    }

    /// <summary>
    /// Remove a pauseable item reference.
    /// </summary>
    /// <param name="pauseable">Class that implements IPauseable.</param>
    public void RemovePauseable(IPauseable pauseable)
    {
        _pauseables.Remove(pauseable);
    }
}