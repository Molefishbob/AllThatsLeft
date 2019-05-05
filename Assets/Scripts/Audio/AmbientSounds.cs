using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSounds : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum time interval between sounds in seconds")]
    private float _maxTimeInterval = 120;
    [SerializeField, Tooltip("Minimum time interval between sounds in seconds")]
    private float _minTimeInterval = 30;
    private RandomSFXSound _sounds = null;
    private ScaledOneShotTimer _timer = null;

    private void Awake()
    {
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Start()
    {
        _sounds = GetComponentInChildren<RandomSFXSound>();
        _timer.StartTimer((int)Random.Range(_minTimeInterval, _maxTimeInterval));
        _timer.OnTimerCompleted += PlaySound;
    }

    private void PlaySound()
    {
        _sounds.PlaySound();
        _timer.StartTimer((int)Random.Range(_minTimeInterval, _maxTimeInterval));
    }

    private void OnDisable()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= PlaySound;
        }
    }
}
