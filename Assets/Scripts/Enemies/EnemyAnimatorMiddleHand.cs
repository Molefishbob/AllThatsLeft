using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorMiddleHand : MonoBehaviour
{
    [SerializeField]
    private RandomSFXSound _hopsfx;

    public void PlayHopSound()
    {
        _hopsfx.PlaySound(true);
    }
}
