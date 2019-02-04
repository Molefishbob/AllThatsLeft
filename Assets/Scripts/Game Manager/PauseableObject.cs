using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PauseableObject : MonoBehaviour, IPauseable {
    protected bool _paused;
    protected virtual void Start() {
        _paused = GameManager.Instance.Paused;
        AddToPauseCollection();
    }
    protected virtual void OnDestroy() {
        RemoveFromPauseCollection();
    }
    public virtual void Pause() {
        _paused = true;
    }
    public virtual void UnPause() {
        _paused = false;
    }
    public void AddToPauseCollection() {
        GameManager.Instance.Pauseables.Add(this);
    }
    public void RemoveFromPauseCollection() {
        if(GameManager.Instance != null) GameManager.Instance.Pauseables.Remove(this);
    }
}