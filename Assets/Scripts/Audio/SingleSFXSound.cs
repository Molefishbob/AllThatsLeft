using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSFXSound : SingleUISound, IPauseable {
    protected bool _paused;
    protected override void Start() {
        base.Start();
        _paused = GameManager.Instance.Paused;
        if(_paused) _audioSource.Pause();
        AddToPauseCollection();
    }
    protected override void OnDestroy() {
        base.OnDestroy();
        RemoveFromPauseCollection();
    }
    public override void PlaySound(bool usePitch) {
        if(!_paused) base.PlaySound(usePitch);
    }
    protected override void SetSoundType() {
        _soundType = SoundType.SoundEffect;
    }
    public virtual void Pause() {
        _paused = true;
        _audioSource.Pause();
    }
    public virtual void UnPause() {
        _paused = false;
        _audioSource.UnPause();
    }
    public void AddToPauseCollection() {
        GameManager.Instance.Pauseables.Add(this);
    }
    public void RemoveFromPauseCollection() {
        if(GameManager.Instance != null) GameManager.Instance.Pauseables.Remove(this);
    }
}