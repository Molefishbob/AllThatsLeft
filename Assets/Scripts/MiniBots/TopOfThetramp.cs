using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopOfThetramp : MonoBehaviour
{
    public float _fJumpHeight = 5;
    private PlayerJump _PlayerMover;
    private MainCharMovement _Player;
    private CharControlPlatformMovement _PlayerCCPM;
    private CharControlPlatformMovement _ownCCPM;
    private Animator _animator;
    [HideInInspector]
    public SingleSFXSound _bounceSound = null;

    void Awake()
    {
        _animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
        _ownCCPM = transform.parent.GetComponent<CharControlPlatformMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_PlayerMover == null)
        {
            _PlayerMover = other.GetComponent<PlayerJump>();
            _Player = other.GetComponent<MainCharMovement>();
            _PlayerCCPM = other.GetComponent<CharControlPlatformMovement>();
        }
        if (!_Player.IsGrounded && !_Player.ControlsDisabled)
        {
            _bounceSound.PlaySound(false);
            _animator.SetTrigger("TrampolineBounce");
            _PlayerMover.ForceJump(_fJumpHeight);
            if (_ownCCPM._platform != null) _PlayerCCPM.ForcePlatform(_ownCCPM._platform);
        }
    }
}
