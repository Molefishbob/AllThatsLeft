using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Transform _scaledObject = null;
    [SerializeField]
    private float _startScale = 0f;
    [SerializeField]
    private float _endScale = 1f;
    [SerializeField]
    private float _timeBeforeTransition = 1f;
    [SerializeField]
    private float _transitionDuration = 2f;

    private UnscaledOneShotTimer _timer;
    private bool _mute;
    private bool _inTransition;

    private void Awake()
    {
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
    }

    private void OnEnable()
    {
        _scaledObject.localScale = Vector3.zero;

        _mute = PrefsManager.Instance.AudioMuteSFX;
        PrefsManager.Instance.AudioMuteSFX = true;

        if (GameManager.Instance.Player != null)
            GameManager.Instance.Player.ControlsDisabled = true;

        _inTransition = false;

        _timer.OnTimerCompleted += GrowUp;
        _timer.StartTimer(_timeBeforeTransition);
    }

    private void Update()
    {
        if (_inTransition)
        {
            float currentScale = _timer.NormalizedTimeElapsed * (_endScale - _startScale) + _startScale;
            _scaledObject.localScale = Vector3.one * currentScale;
        }
    }

    private void GrowUp()
    {
        _timer.OnTimerCompleted -= GrowUp;

        _scaledObject.localScale = Vector3.one * _startScale;

        PrefsManager.Instance.AudioMuteSFX = _mute;

        if (GameManager.Instance.Player != null)
            GameManager.Instance.Player.ControlsDisabled = false;

        _inTransition = true;

        _timer.OnTimerCompleted += EndMe;
        _timer.StartTimer(_transitionDuration);
    }

    private void EndMe()
    {
        _timer.OnTimerCompleted -= EndMe;
        gameObject.SetActive(false);
    }
}