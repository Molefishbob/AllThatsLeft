using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSFXSound : SingleSFXSound
{
    [SerializeField]
    private string _resourceFolder = "";
    private AudioClip[] _sounds;

    protected override void Awake()
    {
        base.Awake();
        _sounds = Resources.LoadAll<AudioClip>(_resourceFolder);
    }

    public override void PlaySound()
    {
        PlaySound(true, Random.Range(0, _sounds.Length));
    }

    public override void PlaySound(bool usePitch)
    {
        PlaySound(usePitch, Random.Range(0, _sounds.Length));
    }

    public virtual void PlaySound(int index)
    {
        PlaySound(true, index);
    }

    public virtual void PlaySound(bool usePitch, int index)
    {
        if (!_paused)
        {
            if (usePitch)
            {
                RandomizePitch();
            }
            index = Mathf.Clamp(index, 0, _sounds.Length);
            _audioSource.PlayOneShot(_sounds[index]);
        }
    }
}