using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    // (Optional) Prevent non-singleton constructor use.
    protected GameManager() { }
    public HashSet<IPauseable> Pauseables { get; private set; } = new HashSet<IPauseable>();
    public bool Paused { get; private set; }
    private float _timeScaleBeforePause = 1.0f;
    public void PauseGame() {
        Paused = true;
        _timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;
        foreach(IPauseable item in Pauseables) {
            if(item != null) {
                item.Pause();
            }
        }
    }
    public void UnPauseGame() {
        Paused = false;
        Time.timeScale = _timeScaleBeforePause;
        foreach(IPauseable item in Pauseables) {
            if(item != null) {
                item.UnPause();
            }
        }
    }
}