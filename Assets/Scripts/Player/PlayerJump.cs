using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public event GenericEvent OnPlayerJump;

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
            EndAfterLeeway();
        }
    }
    private bool _controlsDisabled;

    private Vector3 _currentJumpForce;
    private bool _jumping;
    private bool _forcedJumping;
    private ScaledOneShotTimer _afterTimer;
    private ScaledOneShotTimer _beforeTimer;
    private ScaledOneShotTimer _holdTimer;
    private bool _canJump;
    private PlayerMovement _character;
    private bool _paused = true;

    private void Awake()
    {
        _afterTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _beforeTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _holdTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _character = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _afterTimer.OnTimerCompleted += EndAfterLeeway;
    }

    private void OnDestroy()
    {
        if (_afterTimer != null)
        {
            _afterTimer.OnTimerCompleted -= EndAfterLeeway;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            _paused = true;
            return;
        }
        if (_paused)
        {
            _paused = false;
            return;
        }

        if (ControlsDisabled || _character.HoldPosition) return;

        if (_jumping)
        {
            if (Input.GetButtonUp(_jumpButton) && _holdTimer.IsRunning)
            {
                _currentJumpForce = GetJumpForce(_minHeight);
            }
        }
        else if (Input.GetButtonDown(_jumpButton))
        {
            _beforeTimer.StartTimer(_leewayTimeBeforeHittingGround);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

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
                EndAfterLeeway();
                _afterTimer.StopTimer();
                _character.ResetGravity();
                _currentJumpForce = GetJumpForce(_maxHeight);
                _holdTimer.StartTimer(_holdTimeForMaxHeight);
                _sound?.PlaySound();
                _character._animator?.SetTrigger(_animatorTriggerJump);

                if (OnPlayerJump != null) OnPlayerJump();
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
        ForceJump(height, true);
    }

    /// <summary>
    /// Force the player to jump a certain height immediately.
    /// </summary>
    /// <param name="height">height in meters</param>
    /// <param name="playAnimation">play jump animation</param>
    public void ForceJump(float height, bool playAnimation)
    {
        _character.ResetGravity();
        _currentJumpForce = GetJumpForce(height);
        _forcedJumping = true;
        if (playAnimation)
        {
            _sound?.PlaySound();
            _character._animator?.SetTrigger(_animatorTriggerJump);
        }
    }

    private void EndAfterLeeway()
    {
        _canJump = false;
    }

    public void ResetJump()
    {
        _jumping = false;
        _forcedJumping = false;
        _beforeTimer.StopTimer();
        _holdTimer.StopTimer();
        _afterTimer.StopTimer();
        EndAfterLeeway();
    }
}
