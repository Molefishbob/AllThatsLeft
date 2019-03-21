using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Current checkpoint
    /// </summary>
    /// <value></value>
    public CheckPointPole _currentCheckPoint { get; private set; }
    private CheckPointPole[] _allLevelCheckPoints;
    public PlayerMovement _playerPrefab;
    public NoZoomThirdPersonCam _cameraPrefab;
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

    public bool _levelNeedsFrogEnemies;
    public bool _levelNeedsPatrolEnemies;

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

        _allLevelCheckPoints = FindObjectsOfType<CheckPointPole>();

        if (GameManager.Instance.Player == null)
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player == null)
            {
                player = Instantiate(_playerPrefab);
            }
            GameManager.Instance.Player = player;
        }

        if (GameManager.Instance.Camera == null)
        {
            NoZoomThirdPersonCam camera = FindObjectOfType<NoZoomThirdPersonCam>();
            if (camera == null)
            {
                camera = Instantiate(_cameraPrefab);
            }
            GameManager.Instance.Camera = camera;
        }
    }

    private void Start() {
        SortCheckpoints();

        if (_allLevelCheckPoints != null && _allLevelCheckPoints.Length > 0)
        {
            SetCheckpoint(_allLevelCheckPoints[0]);
        }

        GameManager.Instance.Player.transform.position = GetSpawnLocation();
        GameManager.Instance.Player.SetControllerActive(true);
    }

    /// <summary>
    /// Gives the spawn location from current checkpoint.
    /// </summary>
    /// <returns>Spawn location</returns>
    public Vector3 GetSpawnLocation()
    {
        if (_currentCheckPoint == null)
            return Vector3.zero;
        else
            return _currentCheckPoint.SpawnPoint.position;
    }

    public void SetCheckpoint(CheckPointPole cp)
    {
        if (_currentCheckPoint == null)
        {
            _currentCheckPoint = cp;
        }
        else if (_currentCheckPoint.id < cp.id)
        {
            _currentCheckPoint = cp;
        }
    }

    private void SortCheckpoints()
    {
        bool sorted = false;
        while (!sorted)
        {
            bool swapped = false;
            for (int i = 0; i < _allLevelCheckPoints.Length - 1; i++)
            {
                if (_allLevelCheckPoints[i].id > _allLevelCheckPoints[i + 1].id)
                {
                    CheckPointPole tmp = _allLevelCheckPoints[i];
                    _allLevelCheckPoints[i] = _allLevelCheckPoints[i + 1];
                    _allLevelCheckPoints[i + 1] = tmp;
                    swapped = true;
                }
                else if (_allLevelCheckPoints[i].id == _allLevelCheckPoints[i + 1].id)
                {
                    Debug.LogError("THERE ARE 2 OR MORE CHECKPOINTS WITH SAME ID!! FIX!!");
                    swapped = false;
                    break;
                }
            }
            sorted = !swapped;
        }
    }
}
