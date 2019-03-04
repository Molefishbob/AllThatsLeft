using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampBot : GenericBot
{
    public float _fMaxJumpHeight = 10;
    const int _iPlayerLayer = 10;
    private PlayerJump _PlayerMover;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _iPlayerLayer){
            _PlayerMover = other.GetComponent<PlayerJump>();
            _PlayerMover.ForceJump(_fMaxJumpHeight);
        }
    }
}
