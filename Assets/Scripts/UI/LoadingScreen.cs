using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private int _loadState = -1;

    private void Awake()
    {
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
        _teleportTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _shaderProperty = Shader.PropertyToID("_cutoff");
    }

    private void OnEnable()
    {
        _scaledObject.localScale = Vector3.zero;
        _inTransition = false;
        _loadState = -1;
        _mute = PrefsManager.Instance.AudioMuteSFX;
        PrefsManager.Instance.AudioMuteSFX = true;
        SceneManager.sceneLoaded += Begin;
    }

    private void Start()
    {
        _teleportTimer.OnTimerCompleted += TeleportDone;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Begin;
    }

    private void OnDestroy()
    {
        if (_teleportTimer != null) _teleportTimer.OnTimerCompleted -= TeleportDone;
    }

    private void Update()
    {
        if (_loadState < 0) return;

        if (_loadState < 1)
        {
            _loadState++;
            return;
        }

        if (_loadState == 1)
        {
            _timer.OnTimerCompleted += GrowUp;
            _timer.StartTimer(_timeBeforeTransition);
            _loadState++;
            return;
        }

        if (_inTransition)
        {
            float currentScale = _timer.NormalizedTimeElapsed * (_endScale - _startScale) + _startScale;
            _scaledObject.localScale = Vector3.one * currentScale;

            if (!_teleportTimer.IsRunning && _timer.NormalizedTimeElapsed >= _normalizedEffectStartTime)
            {
                _teleportTimer.StartTimer(_teleportEffectTime);
                GameManager.Instance.Player._teleportEffectFast.Play();
                PrefsManager.Instance.AudioMuteSFX = _mute;
            }
        }

        if (_teleportTimer.IsRunning)
        {
            GameManager.Instance.Player._renderer.material.SetFloat(_shaderProperty, _teleportTimer.NormalizedTimeElapsed);
        }
    }

    private void Begin(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.Camera.PlayerControlled = false;
        GameManager.Instance.Player.ControlsDisabled = true;
        GameManager.Instance.Player._renderer.material.SetFloat(_shaderProperty, 0.0f);

        _loadState = 0;
    }

    private void GrowUp()
    {
        _timer.OnTimerCompleted -= GrowUp;

        _scaledObject.localScale = Vector3.one * _startScale;

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
