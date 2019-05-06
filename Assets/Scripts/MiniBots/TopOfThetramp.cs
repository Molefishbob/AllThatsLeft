using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopOfThetramp : MonoBehaviour
{
    public float _fJumpHeight = 5;
    private PlayerJump _PlayerMover;
    private MainCharMovement _Player;
    private Animator _animator;

    void Awake()
    {
        _animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_PlayerMover == null)
        {
            _PlayerMover = other.GetComponent<PlayerJump>();
            _Player = other.GetComponent<MainCharMovement>();
        }
        if (!_Player.IsGrounded && !_Player.ControlsDisabled)
        {
            _animator.SetTrigger("TrampolineBounce");
            _PlayerMover.ForceJump(_fJumpHeight);
        }
    }
}
