using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollection : VolumeControl {
    [SerializeField]
    private string _resourceFolder = "";
    [SerializeField]
    private float _pitchVariance = 0.25f;
    private AudioClip[] _sounds;
    public bool IsPlaying { get { return _audioSource.isPlaying; } }
    private void Awake() {
        _sounds = Resources.LoadAll<AudioClip>(_resourceFolder);
        _audioSource = GetComponent<AudioSource>();
    }
    public void PlaySound() {
        PlaySound(Random.Range(0,_sounds.Length), true);
    }
    public void PlaySound(int index) {
        PlaySound(index, true);
    }
    public void PlaySound(bool usePitch) {
        PlaySound(Random.Range(0,_sounds.Length), true);
    }
    public void PlaySound(int index, bool usePitch) {
        index = Mathf.Clamp(index,0,_sounds.Length);
        if(usePitch && _pitchVariance != 0) _audioSource.pitch = 1 + Random.Range(-_pitchVariance,_pitchVariance);
        else _audioSource.pitch = 1;
        _audioSource.PlayOneShot(_sounds[index]);
    }
}