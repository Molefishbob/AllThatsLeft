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
    public float _teleportTime = 2;
    private ScaledOneShotTimer _timer;
    private ScaledOneShotTimer _shaderTimer;
    private int _shaderProperty;

    public List<Transform> Targets
    {
        get { return _patrolTargets; }
    }

    private void Awake()
    {
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
        _shaderTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _timer.OnTimerCompleted += Spawn;
        _shaderTimer.OnTimerCompleted += TeleportComplete;
        _shaderProperty = Shader.PropertyToID("_cutoff");
        foreach (Transform child in transform)
        {
            _patrolTargets.Add(child);
        }
    }

    private void Update()
    {
        if (_shaderTimer.IsRunning)
        {
            Debug.Log("aa");
            _patrolEnemy._renderer.materials[1].SetFloat(_shaderProperty, _shaderTimer.NormalizedTimeElapsed);
        }
    }

    private void OnDisable()
    {
        _patrolEnemy._renderer.materials[1].SetFloat(_shaderProperty, 0.0f);
    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= Spawn;
        if (_shaderTimer != null) _shaderTimer.OnTimerCompleted -= TeleportComplete;
    }

    public void Spawn()
    {
        _patrolEnemy = GameManager.Instance.PatrolEnemyPool.GetObject();
        _patrolEnemy.Speed = _speed;
        _patrolEnemy.transform.position = transform.position;
        _patrolEnemy.Targets = _patrolTargets;
        _patrolEnemy._spawner = this;
        _shaderTimer.StartTimer(_teleportTime);
    }

    private void TeleportComplete()
    {
        Debug.Log("ss");
        _patrolEnemy.SetControllerActive(true);
        _patrolEnemy.gameObject.GetComponentInChildren<ScorpionTakeDamage>()._scorpion._renderer.materials[0].SetFloat(_shaderProperty, 0.0f);
        _patrolEnemy._renderer.materials[1].SetFloat(_shaderProperty, 0.0f);
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