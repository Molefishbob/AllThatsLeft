using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour, IDamageReceiver
{
    [SerializeField]
    private string _deadBool = "Dead";
    private EnemyMover _frog;
    public ParticleSystem _dissolveFlakes;
    private int _shaderProperty;
    protected ScaledOneShotTimer _timer;
    [SerializeField]
    private float _dissolveTime = 4.0f;

    public bool Dead { get; protected set; }

    private void Awake()
    {
        _frog = GetComponent<EnemyMover>();
        _shaderProperty = Shader.PropertyToID("_cutoff");
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
        _timer.OnTimerCompleted += DeathComplete;
    }
    private void Start()
    {
        var main = _dissolveFlakes.main;
        main.duration = _dissolveTime;
    }

    private void OnDisable()
    {
        Dead = false;
        _timer.StopTimer();
        _frog._renderer.material.SetFloat(_shaderProperty, 0.0f);
    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= DeathComplete;
    }

    private void Update()
    {
        if (_timer.IsRunning)
        {
            _frog._renderer.material.SetFloat(_shaderProperty, _timer.NormalizedTimeElapsed);
        }
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        if (Dead) return;
        Dead = true;
        _frog.SetControllerActive(false);
        if (_frog._deathSound != null) _frog._deathSound.PlaySound();
        _frog._attack.gameObject.SetActive(false);
        _frog._animator.SetBool(_deadBool, true);
        _timer.StartTimer(_dissolveTime);
        _dissolveFlakes.Play();
    }

    private void DeathComplete()
    {
        if (_frog != null)
        {
            _frog._animator.SetBool(_deadBool, false);
            _frog.gameObject.SetActive(false);
        }
    }
}
