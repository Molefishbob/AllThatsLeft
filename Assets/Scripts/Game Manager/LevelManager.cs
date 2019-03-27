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

    public bool _levelNeedsFrogEnemies;
    public bool _levelNeedsPatrolEnemies;
    [SerializeField]
    private string _pauseMenuButton = "Pause Game";

    void Awake()
    {
        PrefsManager.Instance.Level = SceneManager.GetActiveScene().buildIndex;
        PrefsManager.Instance.Save();

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
            }
            else
            {
                GameManager.Instance.Player = players[0];
            }
            DontDestroyOnLoad(GameManager.Instance.Player);
        }
        foreach (MainCharMovement p in players)
        {
            if (p == GameManager.Instance.Player) continue;
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
    }

    private void Start()
    {
        if (_allLevelCheckPoints != null && _allLevelCheckPoints.ContainsKey(0))
        {
            SetCheckpointByID(0);
        }
        else
        {
            Debug.LogError("Spawn is missing.");
        }

        GameManager.Instance.Player.transform.position = GetSpawnPosition();
        GameManager.Instance.Player.transform.rotation = GetSpawnRotation();
        GameManager.Instance.Player.SetControllerActive(true);
        GameManager.Instance.Camera.GetInstantNewTarget(GameManager.Instance.Player.transform);

        GameManager.Instance.ActivateGame(true);
    }

    private void Update()
    {
        if (Input.GetButtonDown(_pauseMenuButton))
        {
            bool currentState = GameManager.Instance.PauseMenu.gameObject.activeSelf;

            switch (currentState)
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

    public void SetCheckpointByID(int id)
    {
        if (!_allLevelCheckPoints.ContainsKey(id))
        {
            Debug.LogWarning("That checkpoint ID doesn't exist.");
            return;
        }

        if (_currentCheckPoint == null || id > _currentCheckPoint.id)
        {
            _allLevelCheckPoints.TryGetValue(id, out _currentCheckPoint);
            PrefsManager.Instance.CheckPoint = _currentCheckPoint.id;
            PrefsManager.Instance.Save();
        }
    }

    public void SetCheckpoint(CheckPointPole cp)
    {
        if (!_allLevelCheckPoints.ContainsValue(cp))
        {
            Debug.LogError("That checkpoint is missing from collection of all checkpoints. This is very very bad.");
            return;
        }

        if (_currentCheckPoint == null || cp.id > _currentCheckPoint.id)
        {
            _currentCheckPoint = cp;
            PrefsManager.Instance.CheckPoint = _currentCheckPoint.id;
            PrefsManager.Instance.Save();
        }
    }

    public void ResetLevel()
    {
        GameManager.Instance.BotPool.ResetPool();
        GameManager.Instance.FrogEnemyPool?.ResetPool();
        GameManager.Instance.PatrolEnemyPool?.ResetPool();
        DontDestroyOnLoad(gameObject);
        GameManager.Instance.ReloadScene();
        GameManager.Instance.UndoDontDestroy(gameObject);
        Awake();
        Start();
    }
}
