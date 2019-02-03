using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : VolumeControl {
    [SerializeField]
    protected float _pitchVariance = 0.25f;
    public bool IsPlaying { get { return _audioSource.isPlaying; } }
    protected virtual void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }
    protected void RandomizePitch() {
        if(_pitchVariance != 0) {
            _audioSource.pitch = 1 + Random.Range(-_pitchVariance, _pitchVariance);
        } else {
            _audioSource.pitch = 1;
        }
    }
    public virtual void PlaySound() {
        PlaySound(true);
    }
    public virtual void PlaySound(bool usePitch) {
        if(usePitch) {
            RandomizePitch();
        }
        _audioSource.Play();
    }
}