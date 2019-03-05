using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour, IPauseable, ITimedAction
{
    public float _jumpHeight;
    public string _jumpButton = "Jump";
    public float _jumpButtonLeeway;

    private Vector3 _currentJumpForce;
    private bool _jumping;
    private bool _forcedJumping;
    private ThirdPersonPlayerMovement _player;
    private OneShotTimer _timer;
    private bool _pressedJump;

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
        _timer = GetComponent<OneShotTimer>();
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
        _timer.SetTimerTarget(this);
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
            if (Input.GetButtonDown(_jumpButton))
            {
                _pressedJump = true;
                _timer.StartTimer(_jumpButtonLeeway);
            }

            if (_forcedJumping)
            {
                _forcedJumping = false;
                _jumping = true;
            }
            else if (_player.IsGrounded)
            {
                if (_pressedJump)
                {
                    _jumping = true;
                    _currentJumpForce = GetJumpForce(_jumpHeight);
                }
                else
                {
                    _jumping = false;
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

    public void TimedAction()
    {
        _pressedJump = false;
    }
}