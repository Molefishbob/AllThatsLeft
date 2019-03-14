using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour, IPauseable, ITimedAction
{
    [SerializeField]
    private float _maxHeight = 2.5f;
    [SerializeField]
    private float _minHeight = 1.0f;
    [SerializeField]
    private string _jumpButton = "Jump";
    [SerializeField]
    private float _leewayTimeAfterLeavingGround = 0.1f;
    [SerializeField]
    private float _leewayTimeBeforeHittingGround = 0.1f;
    [SerializeField]
    private float _holdTimeForMaxHeight = 0.5f;
    [SerializeField]
    private RandomSFXSound _sound = null;

    private Vector3 _currentJumpForce;
    private bool _jumping;
    private bool _forcedJumping;
    private OneShotTimer _afterTimer;
    private OneShotTimer _beforeTimer;
    private OneShotTimer _holdTimer;
    private bool _canJump;

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
        OneShotTimer[] timers = GetComponents<OneShotTimer>();
        _afterTimer = timers[0];
        _beforeTimer = timers[1];
        _holdTimer = timers[2];
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
        _afterTimer.SetTimerTarget(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    private void Update()
    {
        if (!_paused)
        {
            if (_jumping)
            {
                if (Input.GetButtonUp(_jumpButton) && _holdTimer.IsRunning)
                {
                    _currentJumpForce = GetJumpForce(_minHeight);
                }
            }
            else if (Input.GetButtonDown(_jumpButton))
            {
                _beforeTimer.StartTimer(_leewayTimeBeforeHittingGround, false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_paused)
        {
            if (!_forcedJumping)
            {
                if (!_jumping)
                {
                    if (GameManager.Instance.Player.IsGrounded)
                    {
                        _canJump = true;
                    }
                    else if (_canJump == true && !_afterTimer.IsRunning)
                    {
                        _afterTimer.StartTimer(_leewayTimeAfterLeavingGround);
                    }
                }

                if (_canJump && _beforeTimer.IsRunning)
                {
                    _beforeTimer.StopTimer();
                    _jumping = true;
                    TimedAction();
                    _afterTimer.StopTimer();
                    GameManager.Instance.Player.ResetGravity();
                    _currentJumpForce = GetJumpForce(_maxHeight);
                    _holdTimer.StartTimer(_holdTimeForMaxHeight, false);
                    _sound.PlaySound();
                }
            }

            if (_jumping || _forcedJumping)
            {

                if (_currentJumpForce.magnitude * Time.deltaTime < GameManager.Instance.Player.CurrentGravity)
                {
                    GameManager.Instance.Player.ResetGravity();
                    _jumping = false;
                    _forcedJumping = false;
                }
                else
                {
                    GameManager.Instance.Player.AddDirectMovement(_currentJumpForce);
                }
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
        GameManager.Instance.Player.ResetGravity();
        _currentJumpForce = GetJumpForce(height);
        _forcedJumping = true;
    }

    public void TimedAction()
    {
        _canJump = false;
    }
}