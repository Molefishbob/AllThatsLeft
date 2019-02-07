using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IButtonInteraction
{
    public string _openTrigger = "Open";
    public string _closeTrigger = "Close";
    private Animator _anim;

    // Awake is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void ButtonDown()
    {
        _anim.SetTrigger(_openTrigger);
    }

    public void ButtonUp()
    {
        _anim.SetTrigger(_closeTrigger);
    }
}
