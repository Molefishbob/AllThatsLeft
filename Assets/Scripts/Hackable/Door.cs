using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IButtonInteraction
{
    [Tooltip("The name of the 'opening' trigger parameter in the animator")]
    public string _openBool = "Open";
    private Animator _anim;

    // Awake is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void ButtonDown()
    {
        _anim.SetBool(_openBool,true);
    }

    public void ButtonUp()
    {
        _anim.SetBool(_openBool,false);
    }
}
