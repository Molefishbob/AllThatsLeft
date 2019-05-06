using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private CheckPointPole _currentCheckPoint;
    private Dictionary<int, CheckPointPole> _allLevelCheckPoints;
    public MainCharMovement _playerPrefab;
    public ThirdPersonCamera _cameraPrefab;
    public RotateSky _skyCameraPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public ControlledBotPool _botPoolPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public FrogEnemyPool _frogPoolPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public PatrolEnemyPool _patrolEnemyPoolPrefab;
    public PauseMenu _pauseMenu;
    public LoadingScreen _loadingScreen;
    public LoopingMusic _levelMusic;

    public bool _levelNeedsFrogEnemies;
    public bool _levelNeedsPatrolEnemies;
    [SerializeField]
    private string _pauseMenuButton = "Pause Game";

    private bool _playerInScene = false;

    void Awake()
    {
        GameManager.Instance.LevelManager = this;

        if (GameManager.Instance.BotPool == null)
        {
            GameManager.Instance.BotPool = Instantiate(_botPoolPrefab);
        }

        if (_levelNeedsFrogEnemies && GameManager.Instance.FrogEnemyPool == null)
        {
            GameManager.Instance.FrogEnemyPool = Instantiate(_frogPoolPrefab);
        }
        if (_levelNeedsPatrolEnemies && GameManager.Instance.PatrolEnemyPool == null)
        {
            GameManager.Instance.PatrolEnemyPool = Instantiate(_patrolEnemyPoolPrefab);
        }

        CheckPointPole[] foundPoints = FindObjectsOfType<CheckPointPole>();
        if (foundPoints != null)
        {
            _allLevelCheckPoints = new Dictionary<int, CheckPointPole>(foundPoints.Length);
            foreach (CheckPointPole cp in foundPoints)
            {
                if (_allLevelCheckPoints.ContainsKey(cp.id))
                {
                    Debug.LogError("THERE ARE 2 OR MORE CHECKPOINTS WITH ID " + cp.id + " !! FIX!!");
                    continue;
                }
                _allLevelCheckPoints.Add(cp.id, cp);
            }
        }

        MainCharMovement[] players = FindObjectsOfType<MainCharMovement>();
        if (GameManager.Instance.Player == null)
        {
            if (players == null || players.Length <= 0)
            {
                GameManager.Instance.Player = Instantiate(_playerPrefab);
                _playerInScene = false;
            }
            else
            {
                GameManager.Instance.Player = players[0];
                _playerInScene = true;
            }
            DontDestroyOnLoad(GameManager.Instance.Player);
        }
        foreach (MainCharMovement p in players)
        {
            if (p == GameManager.Instance.Player) continue;
            if (!_playerInScene)
            {
                _playerInScene = true;
                GameManager.Instance.Player.transform.position = p.transform.position;
                GameManager.Instance.Player.transform.rotation = p.transform.rotation;
            }
            Destroy(p.gameObject);
        }

        if (GameManager.Instance.Camera == null)
        {
            ThirdPersonCamera camera = FindObjectOfType<ThirdPersonCamera>();
            if (camera == null)
            {
                camera = Instantiate(_cameraPrefab);
            }
            DontDestroyOnLoad(camera);
            GameManager.Instance.Camera = camera;
        }

        if (GameManager.Instance.SkyCamera == null)
        {
            RotateSky skycam = FindObjectOfType<RotateSky>();
            if (skycam == null)
            {
                skycam = Instantiate(_skyCameraPrefab);
            }
            DontDestroyOnLoad(skycam);
            GameManager.Instance.SkyCamera = skycam;
        }

        if (GameManager.Instance.PauseMenu == null)
        {
            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu == null)
            {
                pauseMenu = Instantiate(_pauseMenu);
            }
            DontDestroyOnLoad(pauseMenu);
            GameManager.Instance.PauseMenu = pauseMenu;
            pauseMenu.gameObject.SetActive(false);
        }

        if (GameManager.Instance.LoadingScreen == null)
        {
            GameManager.Instance.LoadingScreen = Instantiate(_loadingScreen);
            DontDestroyOnLoad(GameManager.Instance.LoadingScreen);
        }

        if (GameManager.Instance.LevelMusic == null)
        {
            GameManager.Instance.LevelMusic = Instantiate(_levelMusic);
            DontDestroyOnLoad(GameManager.Instance.LevelMusic);
        }
    }

    private void Start()
    {
        if (_allLevelCheckPoints != null && _allLevelCheckPoints.ContainsKey(0))
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            if (currentLevel > 1) PrefsManager.Instance.BotsUnlocked = true; //TODO: remove this
            if (PrefsManager.Instance.Level != currentLevel)
            {
                PrefsManager.Instance.Level = SceneManager.GetActiveScene().buildIndex;
                SetCheckpointByID(0);
            }
            else
            {
                SetCheckpointByID(PrefsManager.Instance.CheckPoint);
            }
        }
        else
        {
            Debug.LogError("Spawn is missing.");
        }

        if (!_playerInScene)
        {
            GameManager.Instance.Player.transform.position = GetSpawnPosition();
            GameManager.Instance.Player.transform.rotation = GetSpawnRotation();
        }
        GameManager.Instance.Player.SetControllerActive(true);
        GameManager.Instance.Camera.MoveToTargetInstant(GameManager.Instance.Player.transform);
        GameManager.Instance.Camera.OnPlayerRebirth();

        GameManager.Instance.ActivateGame(true);
    }

    private void Update()
    {
        if (!GameManager.Instance.LoadingScreen.gameObject.activeSelf && Input.GetButtonDown(_pauseMenuButton))
        {
            switch (GameManager.Instance.GamePaused)
            {
                case false:
                    GameManager.Instance.PauseMenu.gameObject.SetActive(true);
                    GameManager.Instance.PauseMenu.ToPauseMenu();
                    GameManager.Instance.PauseGame();
                    break;
                case true:
                    GameManager.Instance.PauseMenu.gameObject.SetActive(false);
                    GameManager.Instance.UnPauseGame();
                    break;
            }

        }
    }

    private Vector3 GetSpawnPosition()
    {
        if (_currentCheckPoint == null)
            return Vector3.zero;
        else
            return _currentCheckPoint.SpawnPoint.position;
    }

    private Quaternion GetSpawnRotation()
    {
        if (_currentCheckPoint == null)
            return Quaternion.identity;
        else
            return _currentCheckPoint.SpawnPoint.rotation;
    }

    public bool SetCheckpointByID(int id)
    {
        if (!_allLevelCheckPoints.ContainsKey(id))
        {
            Debug.LogWarning("That checkpoint ID doesn't exist.");
            return false;
        }

        if (_currentCheckPoint == null || id > _currentCheckPoint.id)
        {
            _allLevelCheckPoints.TryGetValue(id, out _currentCheckPoint);
            PrefsManager.Instance.CheckPoint = _currentCheckPoint.id;
            PrefsManager.Instance.Save();
            return true;
        }

        return false;
    }

    public bool SetCheckpoint(CheckPointPole cp)
    {
        if (!_allLevelCheckPoints.ContainsValue(cp))
        {
            Debug.LogError("That checkpoint is missing from collection of all checkpoints. This is very very bad.");
            return false;
        }

        if (_currentCheckPoint == null || cp.id > _currentCheckPoint.id)
        {
            _currentCheckPoint = cp;
            PrefsManager.Instance.CheckPoint = _currentCheckPoint.id;
            PrefsManager.Instance.Save();
            return true;
        }

        return false;
    }

    public void ResetLevel()
    {
        GameManager.Instance.ReloadScene(true);
    }
}
