using UnityEngine;

public abstract class VolumeControl
{
    protected AudioSource audioSource;
    private float fullVolume;
    protected void Start() {
        fullVolume = audioSource.volume;
    }
    void SetVolume(float volume) {

    }
    void Mute(bool muted) {

    }
}