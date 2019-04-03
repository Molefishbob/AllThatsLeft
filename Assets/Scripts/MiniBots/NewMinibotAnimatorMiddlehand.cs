using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMinibotAnimatorMiddlehand : MonoBehaviour
{
    private BombAction _bombAction = null;
    [SerializeField]
    private RandomSFXSound _WalkSound = null;

    void Awake()
    {
        _bombAction = transform.parent.gameObject.GetComponent<BombAction>();
    }

    public void Explode()
    {
        _bombAction.ExplodeBot();
    }

    public void PlayStepSound()
    {
        _WalkSound.PlaySound(true);
    }
}
