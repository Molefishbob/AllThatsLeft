using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibotAnimatorMiddlehand : MonoBehaviour
{
    private PlayerBotInteractions _pbi = null;
    [SerializeField]
    private RandomSFXSound _jump = null;
    [SerializeField]
    private RandomSFXSound _walk = null;
    [SerializeField]
    private RandomSFXSound _land = null;
    void Awake()
    {
        _pbi = transform.parent.gameObject.GetComponent<PlayerBotInteractions>();
    }

    public void Explode()
    {
        _pbi.ExplodeBot();
    }

    public void PlayStepSound()
    {
        _walk.PlaySound();
    }
}
