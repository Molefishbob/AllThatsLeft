using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampBot : GenericBot
{
    public float _fMaxJumpHeight = 10;
    const int _iPlayerLayer = 10;
    private PlayerJump _PlayerMover;
    private ThirdPersonPlayerMovement _Player;
    private bool _bCanBounce = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _iPlayerLayer && _bCanBounce)
        {
            if (_PlayerMover == null)
            {
                _PlayerMover = other.GetComponent<PlayerJump>();
                _Player = other.GetComponent<ThirdPersonPlayerMovement>();
            }
            if (!_Player.IsGrounded)
                _PlayerMover.ForceJump(_fMaxJumpHeight);
        }
    }

    public override void StartMovement()
    {
        base.StartMovement();
        _bCanBounce = true;
    }

    public override void ResetBot()
    {
        base.ResetBot();
        _bCanBounce = false;
    }
}
