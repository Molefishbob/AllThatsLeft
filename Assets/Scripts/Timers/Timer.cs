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
    protected abstract void CompletedTimer();
    public void SetTimerTarget(ITimedAction timedObject) {
        _timedObject = timedObject;
    }
    public void StartTimer(float duration) {
        if(_timedObject == null) {
            Debug.LogError("Set timer target first, please.");
            return;
        }
        Duration = duration;
        if(Duration <= 0.0f) {
            Debug.LogWarning("Timer duration is " + Duration + ", are you sure this is what you wanted?");
        }
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