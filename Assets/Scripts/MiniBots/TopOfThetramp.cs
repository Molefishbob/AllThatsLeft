using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopOfThetramp : MonoBehaviour
{
    public float MaxJumpHeight { private get; set; }
    const int _iPlayerLayer = 10;
    private PlayerJump _PlayerMover;
    private ThirdPersonPlayerMovement _Player;
    public bool CanBounce { private get { return _bCanBounce; } set { _bCanBounce = value; } }
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
                _PlayerMover.ForceJump(MaxJumpHeight);
        }
    }
}
