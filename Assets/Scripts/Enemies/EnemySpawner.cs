using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPauseable, ITimedAction
{
    private bool _paused = false;
    private FrogEnemyPool _frogPool;
    private PatrolEnemyPool _patrolPool;
    private FrogEnemy _frogEnemy;
    private PatrolEnemy _patrolEnemy;
    public float _spawnTime = 10f;
    private OneShotTimer _timer;
    public SpawnedEnemy _spawnedEnemy;

    public enum SpawnedEnemy
    {
        Frog,
        Patrol
    };

    private void Awake()
    {
        _frogPool = FindObjectOfType<FrogEnemyPool>();
        _patrolPool = FindObjectOfType<PatrolEnemyPool>();
        _timer = GetComponent<OneShotTimer>();
        _timer.SetTimerTarget(this);
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
        Spawn();
    }

    public void Spawn()
    {
        if (_spawnedEnemy == SpawnedEnemy.Frog)
        {
            _frogEnemy = _frogPool.GetObject();
        }
        else if (_spawnedEnemy == SpawnedEnemy.Patrol)
        {
            _patrolEnemy = _patrolPool.GetObject();
        }
    }

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    public void TimedAction()
    {
        Spawn();
    }

    public void StartTime()
    {
        _timer.StartTimer(_spawnTime);
    }
}