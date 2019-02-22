using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour, IPauseable
{
    public float _jumpHeight;
    public string _jumpButton = "Jump";

    private Vector3 _currentJumpForce;
    private bool _jumping;
    private bool _forcedJumping;
    private ThirdPersonPlayerMovement _player;

    private bool _paused;

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    private void Awake()
    {
        _player = GetComponent<ThirdPersonPlayerMovement>();
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    private void FixedUpdate()
    {
        if (!_paused)
        {
            if (_forcedJumping)
            {
                _forcedJumping = false;
                _jumping = true;
                _player.AddDirectMovement(_currentJumpForce);
            }
            else if (_player.IsGrounded)
            {
                _jumping = Input.GetButton(_jumpButton);
                if (_jumping)
                {
                    _currentJumpForce = GetJumpForce(_jumpHeight);
                }
            }

            if (_jumping)
            {
                _player.AddDirectMovement(_currentJumpForce);
            }
        }
    }

    private Vector3 GetJumpForce(float height)
    {
        return -Physics.gravity.normalized * Mathf.Sqrt(2.0f * height * Physics.gravity.magnitude);
    }

    /// <summary>
    /// Force the player to jump a certain height immediately.
    /// </summary>
    /// <param name="height">height in meters</param>
    public void ForceJump(float height)
    {
        _player.ResetGravity();
        _currentJumpForce = GetJumpForce(height);
        _forcedJumping = true;
    }
}