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
    private ScaledOneShotTimer _dissolveSoundTimer;
    private AudioSource _deathAudioSource;

    public bool Dead { get; protected set; }

    private void Awake()
    {
        _frog = GetComponent<EnemyMover>();
        _shaderProperty = Shader.PropertyToID("_cutoff");
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
        _timer.OnTimerCompleted += DeathComplete;
        _dissolveSoundTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _dissolveSoundTimer.OnTimerCompleted += PlayDissolveSound;
        _deathAudioSource = gameObject.transform.Find("Sounds").gameObject.transform.Find("Frog Death Sound").gameObject.GetComponent<AudioSource>();
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
        if (_dissolveSoundTimer != null) _dissolveSoundTimer.OnTimerCompleted -= PlayDissolveSound;
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
        if (_frog._dissolveSound != null) _dissolveSoundTimer.StartTimer(_deathAudioSource.clip.length);
        _frog._attack.gameObject.SetActive(false);
        _frog._animator.SetBool(_deadBool, true);
        _timer.StartTimer(_dissolveTime);
        _dissolveFlakes.Play();
        _frog._spawner.gameObject.SetActive(false);
    }

    private void DeathComplete()
    {
        if (_frog != null)
        {
            _frog._animator.SetBool(_deadBool, false);
            _frog.gameObject.SetActive(false);
        }
    }

    private void PlayDissolveSound()
    {
        _frog._dissolveSound.PlaySound();
    }
}
