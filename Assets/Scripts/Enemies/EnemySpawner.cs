using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private FrogEnemyPool _frogPool;
    private PatrolEnemyPool _patrolPool;
    private FrogEnemy _frogEnemy;
    private PatrolEnemy _patrolEnemy;
    public int _maxSpawnAmount;
    private int _spawnedCount = 0;
    private bool _hasSpawnedEnough;
    public SpawnedEnemy _spawnedEnemy;
    private List<Transform> _patrolTargets = new List<Transform>();

    public enum SpawnedEnemy
    {
        Frog,
        Patrol
    };

    public List<Transform> Targets
    {
        get { return _patrolTargets; }
    }

    private void Awake()
    {
        if (_spawnedEnemy == SpawnedEnemy.Patrol)
        {
            foreach (Transform child in transform)
            {
                _patrolTargets.Add(child);
            }
        }
    }

    private void Start()
    {
        _frogPool = FindObjectOfType<FrogEnemyPool>();
        _patrolPool = FindObjectOfType<PatrolEnemyPool>();
    }

    public void Spawn()
    {
        if (_spawnedEnemy == SpawnedEnemy.Frog)
        {
            _frogEnemy = _frogPool.GetObject();
            _frogEnemy.transform.position = transform.position;
            _frogEnemy.transform.rotation = transform.rotation;
            _frogEnemy.SetSpawnerTransform(transform);
            _spawnedCount++;
        }
        else if (_spawnedEnemy == SpawnedEnemy.Patrol)
        {
            _patrolEnemy = _patrolPool.GetObject();
            _patrolEnemy.transform.position = transform.position;
            _patrolEnemy.Targets = _patrolTargets;
            _spawnedCount++;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (_spawnedCount < _maxSpawnAmount)
            {
                Spawn();
            }
        }
    }
}