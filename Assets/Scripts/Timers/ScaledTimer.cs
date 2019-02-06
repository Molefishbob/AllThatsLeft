using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScaledTimer : Timer, IPauseable {
    private bool _paused;
    protected virtual void Start() {
        _paused = GameManager.Instance.Paused;
        AddToPauseCollection();
    }
    protected void FixedUpdate() {
        if(IsRunning && !_paused) {
            _timer += Time.deltaTime;
            if(_timer >= Duration) {
                _timer = Duration;
                CompletedTimer();
                _timedObject.TimedAction();
            }
        }
    }
    private void OnDestroy() {
        RemoveFromPauseCollection();
    }
    public void Pause() {
        _paused = true;
    }
    public void UnPause() {
        _paused = false;
    }
    public void AddToPauseCollection() {
        GameManager.Instance.Pauseables.Add(this);
    }
    public void RemoveFromPauseCollection() {
        if(GameManager.Instance != null) GameManager.Instance.Pauseables.Remove(this);
    }
}