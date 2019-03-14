using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPauseable
{
    private bool _paused = false;
    private FrogEnemyPool _frogPool;
    private PatrolEnemyPool _patrolPool;
    private GenericEnemy _enemy;
    public int _maxSpawnAmount;
    private int _spawnedCount = 0;
    private bool _hasSpawnedEnough;
    public SpawnedEnemy _spawnedEnemy;

    public enum SpawnedEnemy
    {
        Frog,
        Patrol
    };

    private void Start()
    {
        _frogPool = FindObjectOfType<FrogEnemyPool>();
        _patrolPool = FindObjectOfType<PatrolEnemyPool>();
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    public void Spawn()
    {
        if (_spawnedEnemy == SpawnedEnemy.Frog)
        {
            _enemy = _frogPool.GetObject();
        }
        else if (_spawnedEnemy == SpawnedEnemy.Patrol)
        {
            _enemy = _patrolPool.GetObject();
        }
        _enemy.transform.position = transform.position;
        _enemy.SetControllerActive(true);
        _spawnedCount++;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            if (_spawnedCount < _maxSpawnAmount)
            {
                Spawn();
            }
        }
    }
}