using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionSpawner : MonoBehaviour
{
    private PatrolEnemy _patrolEnemy;
    private bool _hasSpawned = false;
    private List<Transform> _patrolTargets = new List<Transform>();
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
            _patrolEnemy._renderer.materials[1].SetFloat(_shaderProperty, _shaderTimer.NormalizedTimeElapsed);
        }
    }

    private void OnDisable()
    {
        if (_patrolEnemy != null) _patrolEnemy._renderer.materials[1].SetFloat(_shaderProperty, 0.0f);
    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= Spawn;
        if (_shaderTimer != null) _shaderTimer.OnTimerCompleted -= TeleportComplete;
    }

    private void Spawn()
    {
        _patrolEnemy = GameManager.Instance.PatrolEnemyPool.GetObject();
        _patrolEnemy.transform.position = _patrolTargets[0].position;
        _patrolEnemy.Targets = _patrolTargets;
        _patrolEnemy._spawner = this;
        _patrolEnemy._animator.speed = 0;
        _patrolEnemy.gameObject.GetComponentInChildren<ScorpionTakeDamage>()._scorpion._renderer.materials[0].SetFloat(_shaderProperty, 1.0f);
        _shaderTimer.StartTimer(_teleportTime);
        _patrolEnemy._teleportEffect.Play();
    }

    private void TeleportComplete()
    {
        _patrolEnemy._animator.speed = 1;
        _patrolEnemy.SetControllerActive(true);
        _patrolEnemy.gameObject.GetComponentInChildren<ScorpionTakeDamage>()._scorpion._renderer.materials[0].SetFloat(_shaderProperty, 0.0f);
        _patrolEnemy._renderer.materials[1].SetFloat(_shaderProperty, 0.0f);
        _shaderTimer.StopTimer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasSpawned)
        {
            Spawn();
            _hasSpawned = true;
        }
    }

    public void Respawn()
    {
        _timer.StartTimer(_respawnTime);
    }
}
