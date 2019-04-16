/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private FrogEnemy _frogEnemy;
    private PatrolEnemy _patrolEnemy;
    public int _maxSpawnAmount;
    private int _spawnedCount = 0;
    private bool _hasSpawnedEnough;
    public SpawnedEnemy _spawnedEnemy;
    private List<Transform> _patrolTargets = new List<Transform>();
    [SerializeField]
    private float _speed = 3;

    public enum SpawnedEnemy
    {
        Frog,
        Patrol
    }

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

    public void Spawn()
    {
        if (_spawnedEnemy == SpawnedEnemy.Frog)
        {
            _frogEnemy = GameManager.Instance.FrogEnemyPool.GetObject();
            _frogEnemy.Speed = _speed;
            _frogEnemy.transform.position = transform.position;
            _frogEnemy.transform.rotation = transform.rotation;
            _frogEnemy.SetSpawnerTransform(transform);
            _frogEnemy.SetControllerActive(true);
            _spawnedCount++;
        }
        else if (_spawnedEnemy == SpawnedEnemy.Patrol)
        {
            _patrolEnemy = GameManager.Instance.PatrolEnemyPool.GetObject();
            _patrolEnemy.Speed = _speed;
            _patrolEnemy.transform.position = transform.position;
            _patrolEnemy.Targets = _patrolTargets;
            _patrolEnemy.SetControllerActive(true);
            _spawnedCount++;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (_spawnedCount < _maxSpawnAmount)
        {
            Spawn();
        }
    }
}*/