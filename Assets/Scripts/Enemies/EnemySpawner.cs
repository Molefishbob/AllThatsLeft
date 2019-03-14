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
            _frogEnemy = _frogPool.GetObject();
            _frogEnemy.transform.position = transform.position;
            _frogEnemy.SetControllerActive(true);
            _spawnedCount++;
        }
        else if (_spawnedEnemy == SpawnedEnemy.Patrol)
        {
            _patrolEnemy = _patrolPool.GetObject();
            _patrolEnemy.transform.position = transform.position;

            _spawnedCount++;
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