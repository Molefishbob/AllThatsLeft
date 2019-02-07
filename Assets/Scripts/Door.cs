using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IButtonInteraction
{
    private Animator _anim;

    // Awake is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    
    public void ButtonDown()
    {
        _anim.SetTrigger("Open");
    }

    public void ButtonUp()
    {
        _anim.SetTrigger("Close");
    }
}
