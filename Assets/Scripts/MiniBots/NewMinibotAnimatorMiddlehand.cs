using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMinibotAnimatorMiddlehand : MonoBehaviour
{
    private BombAction _bombAction = null;
    [SerializeField]
    private RandomSFXSound _WalkSound = null;
    private HackAction _hackAction = null;

    void Awake()
    {
        _bombAction = transform.parent.gameObject.GetComponent<BombAction>();
        _hackAction = transform.parent.gameObject.GetComponent<HackAction>();
    }

    public void Explode()
    {
        _bombAction.ExplodeBot();
    }

    public void PlayStepSound()
    {
        _WalkSound.PlaySound(true);
    }

    public void CheckIfHackDone()
    {
        _hackAction.CheckHackDone();
    }
}
