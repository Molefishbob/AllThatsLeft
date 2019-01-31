using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour {
    protected ITimedAction _timedObject;
    protected float _timer = 0.0f;
    private float _duration = 0.0f;
    public float Duration {
        get {
            return _duration;
        }
        protected set {
            if(value < 0.0f) {
                Debug.LogError("Invalid duration!!!");
            } else {
                _duration = value;
            }
        }
    }
    public float TimeElapsed { get { return _timer; } }
    public float TimeLeft { get { return Duration - _timer; } }
    public float NormalizedTimeElapsed { get { return TimeElapsed / Duration; } }
    public float NormalizedTimeLeft { get { return TimeLeft / Duration; } }
    public bool IsRunning { get; protected set; }
    protected abstract void Update();
    public void StartTimer(float duration, ITimedAction timedObject) {
        Duration = duration;
        StartTimer(timedObject);
    }
    public void StartTimer(ITimedAction timedObject) {
        _timedObject = timedObject;
        _timer = 0.0f;
        IsRunning = true;
    }
    public void ResumeTimer() {
        IsRunning = true;
    }
    public void StopTimer() {
        IsRunning = false;
    }
}