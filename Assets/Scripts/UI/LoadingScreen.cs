using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private float _extraLoadingTime = 2.0f;
    private UnscaledOneShotTimer _timer;
    private bool _mute;

    private void Awake()
    {
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
        _timer.OnTimerCompleted += LoadingDone;
    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= LoadingDone;
    }

    private void OnEnable()
    {
        GameManager.Instance.Player.ControlsDisabled = true;
        _mute = PrefsManager.Instance.AudioMuteSFX;
        PrefsManager.Instance.AudioMuteSFX = true;
        _timer.StartTimer(_extraLoadingTime);
    }

    private void LoadingDone()
    {
        GameManager.Instance.Player.ControlsDisabled = false;
        PrefsManager.Instance.AudioMuteSFX = _mute;
        gameObject.SetActive(false);
    }
}