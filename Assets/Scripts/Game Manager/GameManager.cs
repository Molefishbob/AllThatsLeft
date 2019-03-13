using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ValueChanged( int amount );
public delegate void BotChanged( MiniBotType bot );
public enum MiniBotType {
        HackBot,
        BombBot,
        TrampBot
    }
public class GameManager : Singleton<GameManager>
{
    // (Optional) Prevent non-singleton constructor use.
    protected GameManager() { }
    public event ValueChanged OnBotAmountChanged;
    public event ValueChanged OnMaximumBotAmountChanged;
    public event BotChanged OnCurrentBotChanged;
    private HashSet<IPauseable> _pauseables = new HashSet<IPauseable>();

    /// <summary>
    /// Is the game currently paused?
    /// </summary>
    public bool GamePaused { get; private set; }

    private ThirdPersonPlayerMovement _player;

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

    public MiniBotType  CurrentBot {
        get 
        { 
            return _currentBot; 
        }
        set 
        {
            _currentBot = value;

            if (OnCurrentBotChanged != null) {
                OnCurrentBotChanged(_currentBot);
            }
        }
    }

    private MiniBotType _currentBot;
    
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
    public ThirdPersonPlayerMovement Player
    {
        get
        {
            if (_player == null)
            {
                _player = FindObjectOfType<ThirdPersonPlayerMovement>();
                if (_player == null)
                {
                    Debug.LogError("Scene doesn't have an instance of the player.");
                }
            }
            return _player;
        }
    }

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

    public BombPool BombPool;
    public TrampPool TrampPool;
    public HackPool HackPool;

    private float _timeScaleBeforePause = 1.0f;

    private void Awake()
    {

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