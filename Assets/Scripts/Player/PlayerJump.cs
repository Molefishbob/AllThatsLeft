using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour, ITimedAction
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
    [SerializeField]
    private string _animatorTriggerJump = "Jump";

    [HideInInspector]
    public bool ControlsDisabled
    {
        get
        {
            return _controlsDisabled;
        }
        set
        {
            _controlsDisabled = value;
            _afterTimer.StopTimer();
            _beforeTimer.StopTimer();
            _holdTimer.StopTimer();
            TimedAction();
        }
    }
    private bool _controlsDisabled;

    private Vector3 _currentJumpForce;
    private bool _jumping;
    private bool _forcedJumping;
    private OneShotTimer _afterTimer;
    private OneShotTimer _beforeTimer;
    private OneShotTimer _holdTimer;
    private bool _canJump;
    private CharControlBase _character;

    private void Awake()
    {
        OneShotTimer[] timers = GetComponents<OneShotTimer>();
        _afterTimer = timers[0];
        _beforeTimer = timers[1];
        _holdTimer = timers[2];
        _character = GetComponent<CharControlBase>();
    }

    private void Start()
    {
        _afterTimer.SetTimerTarget(this);
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }

        if (!ControlsDisabled)
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
        if (GameManager.Instance.GamePaused)
        {
            return;
        }

        if (!_forcedJumping)
        {
            if (!_jumping)
            {
                if (_character.IsGrounded)
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
                _character.ResetGravity();
                _currentJumpForce = GetJumpForce(_maxHeight);
                _holdTimer.StartTimer(_holdTimeForMaxHeight, false);
                _sound?.PlaySound();
                _character._animator?.SetTrigger(_animatorTriggerJump);
            }
        }

        if (_jumping || _forcedJumping)
        {

            if (_currentJumpForce.magnitude * Time.deltaTime < _character.CurrentGravity)
            {
                _character.ResetGravity();
                _jumping = false;
                _forcedJumping = false;
            }
            else
            {
                _character.AddDirectMovement(_currentJumpForce * Time.deltaTime);
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
        _character.ResetGravity();
        _currentJumpForce = GetJumpForce(height);
        _forcedJumping = true;
    }

    public void TimedAction()
    {
        _canJump = false;
    }
}