using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUISound : SingleUISound
{
    [SerializeField]
    private string _resourceFolder = "";
    private AudioClip[] _sounds;

    protected override void Awake()
    {
        base.Awake();
        _sounds = Resources.LoadAll<AudioClip>(_resourceFolder);
    }

    /// <summary>
    /// Plays random sound effect from the resource folder.
    /// </summary>
    /// <param name="usePitch">Randomize the pitch</param>
    public override void PlaySound(bool usePitch)
    {
        PlaySound(usePitch, Random.Range(0, _sounds.Length));
    }

    /// <summary>
    /// Plays a specific sound effect from the resource folder with randomized pitch.
    /// </summary>
    /// <param name="index">index of the sound effect</param>
    public virtual void PlaySound(int index)
    {
        PlaySound(true, index);
    }

    /// <summary>
    /// Plays a specific sound effect from the resource folder.
    /// </summary>
    /// <param name="usePitch">Randomize the pitch</param>
    /// <param name="index">index of the sound effect</param>
    public virtual void PlaySound(bool usePitch, int index)
    {
        if (usePitch)
        {
            RandomizePitch();
        }
        else
        {
            _audioSource.pitch = _basePitch;
        }
        index = Mathf.Clamp(index, 0, _sounds.Length);
        _audioSource.PlayOneShot(_sounds[index]);
    }
}