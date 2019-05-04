using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionSpawner : MonoBehaviour
{
    private PatrolEnemy _patrolEnemy;
    private bool _hasSpawned = false;
    private List<Transform> _patrolTargets = new List<Transform>();
    [SerializeField]
    private float _speed = 3;
    public float _respawnTime = 10;
    private ScaledOneShotTimer _timer;

    public List<Transform> Targets
    {
        get { return _patrolTargets; }
    }

    private void Awake()
    {
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
        _timer.OnTimerCompleted += Spawn;
        foreach (Transform child in transform)
        {
            _patrolTargets.Add(child);
        }
    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= Spawn;
    }

    public void Spawn()
    {
        _patrolEnemy = GameManager.Instance.PatrolEnemyPool.GetObject();
        _patrolEnemy.Speed = _speed;
        _patrolEnemy.transform.position = transform.position;
        _patrolEnemy.Targets = _patrolTargets;
        _patrolEnemy._spawner = this;
        _patrolEnemy.SetControllerActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasSpawned)
        {
            Spawn();
            _hasSpawned = true;
        }
    }
}