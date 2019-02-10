using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUISound : VolumeControl
{
    [SerializeField]
    protected float _pitchVariance = 0.25f;
    protected float _basePitch;

    protected override void Start()
    {
        base.Start();
        _basePitch = _audioSource.pitch;
    }

    protected void RandomizePitch()
    {
        if (_pitchVariance != 0.0f)
        {
            _audioSource.pitch = _basePitch * (1 + Random.Range(-_pitchVariance, _pitchVariance));
        }
        else
        {
            _audioSource.pitch = _basePitch;
        }
    }

    /// <summary>
    /// Plays the interface sound with randomized pitch.
    /// </summary>
    public virtual void PlaySound()
    {
        PlaySound(true);
    }

    /// <summary>
    /// Plays the interface sound.
    /// </summary>
    /// <param name="usePitch">Randomize the pitch</param>
    public virtual void PlaySound(bool usePitch)
    {
        if (usePitch)
        {
            RandomizePitch();
        }
        _audioSource.Play();
    }

    protected override void SetSoundType()
    {
        _soundType = SoundType.Interface;
    }
}