using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Transform _scaledObject = null;
    [SerializeField]
    private float _startScale = 0.0f;
    [SerializeField]
    private float _endScale = 1.0f;
    [SerializeField]
    private float _timeBeforeTransition = 1.0f;
    [SerializeField]
    private float _transitionDuration = 1.0f;
    [SerializeField]
    private float _normalizedEffectStartTime = 0.1f;
    [SerializeField]
    private float _teleportEffectTime = 1.0f;

    private UnscaledOneShotTimer _timer;
    private ScaledOneShotTimer _teleportTimer;
    private bool _mute;
    private bool _inTransition;
    private int _shaderProperty;

    private void Awake()
    {
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
        _teleportTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _shaderProperty = Shader.PropertyToID("_cutoff");
    }

    private void OnEnable()
    {
        _mute = PrefsManager.Instance.AudioMuteSFX;
        PrefsManager.Instance.AudioMuteSFX = true;

        if (GameManager.Instance.Camera != null)
        {
            GameManager.Instance.Camera.PlayerControlled = false;
        }

        if (GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.ControlsDisabled = true;
            GameManager.Instance.Player._renderer.material.SetFloat(_shaderProperty, 0.0f);
        }

        _inTransition = false;

        _timer.OnTimerCompleted += GrowUp;
        _timer.StartTimer(_timeBeforeTransition);
    }

    private void Start()
    {
        _scaledObject.localScale = Vector3.one * _startScale;
        _teleportTimer.OnTimerCompleted += TeleportDone;
    }

    private void OnDisable()
    {
        _scaledObject.localScale = Vector3.one * _startScale;
    }

    private void OnDestroy()
    {
        if (_teleportTimer != null) _teleportTimer.OnTimerCompleted -= TeleportDone;
    }

    private void Update()
    {
        if (_inTransition)
        {
            float currentScale = _timer.NormalizedTimeElapsed * (_endScale - _startScale) + _startScale;
            _scaledObject.localScale = Vector3.one * currentScale;

            if (!_teleportTimer.IsRunning && _timer.NormalizedTimeElapsed >= _normalizedEffectStartTime)
            {
                _teleportTimer.StartTimer(_teleportEffectTime);
                GameManager.Instance.Player._teleportEffectFast.Play();
            }
        }

        if (_teleportTimer.IsRunning)
        {
            GameManager.Instance.Player._renderer.material.SetFloat(_shaderProperty, _teleportTimer.NormalizedTimeElapsed);
        }
    }

    private void GrowUp()
    {
        _timer.OnTimerCompleted -= GrowUp;

        _scaledObject.localScale = Vector3.one * _startScale;

        PrefsManager.Instance.AudioMuteSFX = _mute;

        _inTransition = true;

        _timer.OnTimerCompleted += EndMe;
        _timer.StartTimer(_transitionDuration);
    }

    private void EndMe()
    {
        _timer.OnTimerCompleted -= EndMe;
        _scaledObject.localScale = Vector3.one * _endScale;
    }

    private void TeleportDone()
    {
        GameManager.Instance.Camera.PlayerControlled = true;
        GameManager.Instance.Player.ControlsDisabled = false;
        GameManager.Instance.Player._renderer.material.SetFloat(_shaderProperty, 1.0f);
        gameObject.SetActive(false);
    }

}
