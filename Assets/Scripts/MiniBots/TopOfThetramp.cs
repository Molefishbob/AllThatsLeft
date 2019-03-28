using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopOfThetramp : MonoBehaviour
{
    public float _fJumpHeight = 5;
    const int _iPlayerLayer = 10;
    private PlayerJump _PlayerMover;
    private PlayerMovement _Player;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _iPlayerLayer)
        {
            if (_PlayerMover == null)
            {
                _PlayerMover = other.GetComponent<PlayerJump>();
                _Player = other.GetComponent<PlayerMovement>();
            }
            if (!_Player.IsGrounded)
                _PlayerMover.ForceJump(_fJumpHeight);
        }
    }
}
