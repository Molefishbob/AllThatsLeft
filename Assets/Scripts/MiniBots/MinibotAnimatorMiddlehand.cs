/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This one is going to be deleted
// I should have written one over this but for some reason i thought that making a new file is better
// Prolly delete this -> rename is going to happen. I hope

public class MinibotAnimatorMiddlehand : MonoBehaviour
{
    private PlayerBotInteractions _pbi = null;
    //[SerializeField]
    //private RandomSFXSound _jump = null;
    [SerializeField]
    private RandomSFXSound _walk = null;
    //[SerializeField]
    //private RandomSFXSound _land = null;
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
 */