using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionTakeDamage : MonoBehaviour, IDamageReceiver
{
    [SerializeField]
    private string _defendtrigger = "Defend";
    private PatrolEnemy _scorpion;
    public ParticleSystem _dissolveFlakes;
    private int _shaderProperty;
    protected ScaledOneShotTimer _timer;
    [SerializeField]
    private float _dissolveTime = 4.0f;

    public bool Dead { get; protected set; }

    private void Awake()
    {
        _scorpion = GetComponentInParent<PatrolEnemy>();
        _shaderProperty = Shader.PropertyToID("_cutoff");
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
        _timer.OnTimerCompleted += DeathComplete;
    }

    private void Start()
    {
        var main = _dissolveFlakes.main;
        main.duration = _dissolveTime;
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    private void OnDisable()
    {
        Dead = false;
        _timer.StopTimer();
        _scorpion._renderer.material.SetFloat(_shaderProperty, 0.0f);

    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= DeathComplete;
    }

    private void Update()
    {
        if (_timer.IsRunning)
        {
            _scorpion._renderer.material.SetFloat(_shaderProperty, _timer.NormalizedTimeElapsed);
        }
    }

    public void Die()
    { 
        if (Dead) return;
        _scorpion._animator.SetTrigger(_defendtrigger);
        _scorpion.StopMoving = true;
        Dead = true;
        _scorpion.SetControllerActive(false);
        //if (_scorpion._deathSound != null) _scorpion._deathSound.PlaySound();
        _scorpion._attack.gameObject.SetActive(false);
        _timer.StartTimer(_dissolveTime);
        _dissolveFlakes.Play();
        _scorpion._dead = true;
    }

    private void DeathComplete()
    {
        if (_scorpion != null)
        {
            _scorpion.gameObject.SetActive(false);
        }
    }
}
