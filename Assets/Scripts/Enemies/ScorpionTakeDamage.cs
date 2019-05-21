using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionTakeDamage : MonoBehaviour, IDamageReceiver
{
    [SerializeField]
    private string _defendtrigger = "Defend";
    [HideInInspector]
    public PatrolEnemy _scorpion;
    public ParticleSystem _dissolveFlakes;
    private int _shaderProperty;
    protected ScaledOneShotTimer _timer;
    [SerializeField]
    private float _dissolveTime = 4.0f;
    private ScaledOneShotTimer _dissolveSoundTimer;
    private AudioSource _deathAudioSource;

    public bool Dead { get; protected set; }

    private void Awake()
    {
        _scorpion = GetComponentInParent<PatrolEnemy>();
        _shaderProperty = Shader.PropertyToID("_cutoff");
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
        _dissolveSoundTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _timer.OnTimerCompleted += DeathComplete;
        _dissolveSoundTimer.OnTimerCompleted += PlayDissolveSound;
        _deathAudioSource = transform.parent.gameObject.transform.Find("Sounds").gameObject.transform.Find("Death").gameObject.GetComponent<AudioSource>();
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
        if (_dissolveSoundTimer != null) _dissolveSoundTimer.OnTimerCompleted -= PlayDissolveSound;
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
        if (_scorpion._walkSound != null) _scorpion._walkSound.StopSound();
        if (_scorpion._deathSound != null) _scorpion._deathSound.PlaySound();
        if (_scorpion._dissolveSound != null) _dissolveSoundTimer.StartTimer(_deathAudioSource.clip.length);
        _scorpion._attack.gameObject.SetActive(false);
        _timer.StartTimer(_dissolveTime);
        _dissolveFlakes.Play();
        _scorpion._dead = true;
    }

    private void DeathComplete()
    {
        if (_scorpion != null)
        {
            _scorpion._spawner.Respawn();
            _scorpion.gameObject.SetActive(false);
        }
    }

    private void PlayDissolveSound()
    {
        _scorpion._dissolveSound.PlaySound();
    }
}
