using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpwardsScale : MonoBehaviour
{
    [SerializeField]
    private float _startScale = 0f;
    [SerializeField]
    private float _endScale = 1f;
    [SerializeField]
    private float _duration = 2f;
    [SerializeField, Tooltip("Disables this object if defined after the timer has runout")]
    private GameObject _disableObject = null;

    private UnscaledOneShotTimer _timer;
    private bool _timerComplete = false;
    private bool _mute;
    private float _currentScale;
    private const int Count = 120;
    private float _dx;

    
    private void OnEnable()
    {

        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
        _timer.StartTimer(_duration);
        _timer.OnTimerCompleted += TimerComplete;
        _mute = PrefsManager.Instance.AudioMuteSFX;
        transform.localScale = new Vector3(_startScale, _startScale, _startScale);
        _currentScale = _startScale;
        _dx = (_endScale - _startScale) / Count;
    }

    // Update is called once per frame
    void Update()
    {
        _currentScale += _dx;
        if (!_timerComplete && _currentScale < _endScale)
        {
            transform.localScale = Vector3.one * _currentScale;
        } else
        {
            transform.localScale = new Vector3(_endScale, _endScale, _endScale);
        }

        if (_timer.TimeElapsed > _timer.TimeElapsed * 0.25f)
        {
            if (GameManager.Instance.Player != null)
                GameManager.Instance.Player.ControlsDisabled = false;
        }
    }

    private void OnDisable()
    {
        _timer.OnTimerCompleted -= TimerComplete;
        _timerComplete = false;
        PrefsManager.Instance.AudioMuteSFX = _mute;
        _currentScale = _startScale;

    }
    private void TimerComplete()
    {
        _timerComplete = true;
        transform.localScale = new Vector3(_startScale, _startScale, _startScale);
        _disableObject.SetActive(false);
    }
}
