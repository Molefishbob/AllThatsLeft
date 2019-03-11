using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPauseable
{
    private bool _paused = false;
    private FrogEnemyPool _frogPool;
    private PatrolEnemyPool _patrolPool;
    private FrogEnemy _frogEnemy;
    private PatrolEnemy _patrolEnemy;
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
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    private void Update()
    {
        if (_spawnedEnemy == SpawnedEnemy.Frog)
        {
            _frogEnemy = _frogPool.GetObject();
            _frogEnemy.transform.position = transform.position;
        }
        else if (_spawnedEnemy == SpawnedEnemy.Patrol)
        {
           _patrolEnemy = _patrolPool.GetObject();
            _patrolEnemy.transform.position = transform.position;
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
}
