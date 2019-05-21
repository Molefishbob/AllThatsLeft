using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointEffectHelper : MonoBehaviour
{
    public ParticleSystem effect = null;

    public void PlayEffect() {
        effect.Play();
    }
}
