using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharControlBase : MonoBehaviour
{
    [SerializeField, Tooltip("Meters per second")]
    protected float _speed = 8;
    [SerializeField, Tooltip("Degrees per second")]
    protected float _turningSpeed = 540;
    [SerializeField, Tooltip("NOT in seconds")]
    private float _accelerationTime = 5;
    [SerializeField, Tooltip("The y position on which the unit dies")]
    private float _minYPosition = -10;
    [SerializeField]
    protected LayerMask _walkableTerrain = (1 << 12) + (1 << 13) + (1 << 14);
    [SerializeField]
    private float _groundedDistanceBonus = 0.22f;
    [SerializeField]
    private RandomSFXSound _landingSound = null;
    [SerializeField]
    private string _animatorBoolRunning = "Run";
    [SerializeField]
    private string _animatorBoolAirborne = "Airborne";
    [SerializeField]
    private bool _startsActive = false;

    [HideInInspector]
    public Animator _animator;
    [HideInInspector]
    public CharacterController _controller;

    private Vector3 _externalMove = Vector3.zero;
    private Vector3 _internalMove = Vector3.zero;
    private Vector3 _currentGravity = Vector3.zero;
    private Vector3 _slopeDirection = Vector3.down;
    private bool _onSlope = false;
    private bool _resetGravity = false;
    private bool _controllerEnabled = true;
    private bool _airBorne = false;

    public bool IsGrounded { get; private set; }
    public float CurrentGravity { get { return _currentGravity.magnitude; } }

    protected virtual void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        SetControllerActive(_startsActive);
    }

    protected virtual void Start()
    {
        _airBorne = !IsGrounded;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_controllerEnabled)
        {
            float maxSpeed = _speed * Time.deltaTime;
            float accelerationMagnitude;
            float decelerationMagnitude;

            if (_accelerationTime > 0)
            {
                accelerationMagnitude = maxSpeed / _accelerationTime;
                decelerationMagnitude = maxSpeed / (_accelerationTime * 2.0f);
            }
            else
            {
                accelerationMagnitude = _speed;
                decelerationMagnitude = _speed;
            }

            // get movement from child class
            Vector3 inputDirection = InternalMovement();

            if (inputDirection.magnitude > 0.0f)
            {
                // convert input direction to a rotation
                Quaternion inputRotation = Quaternion.LookRotation(inputDirection, Vector3.up);

                // rotate character towards input rotation
                if (_turningSpeed > 0.0f)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, inputRotation, _turningSpeed * Time.deltaTime);
                }
                else
                {
                    transform.rotation = inputRotation;
                }

                // save previous move's magnitude
                float previousMoveMagnitude = _internalMove.magnitude;

                // add to current movement
                _internalMove += inputDirection * inputDirection.magnitude * accelerationMagnitude;

                // cap acceleration
                if (_internalMove.magnitude - previousMoveMagnitude > accelerationMagnitude)
                {
                    _internalMove = _internalMove.normalized * (previousMoveMagnitude + accelerationMagnitude);
                }
                // cap deceleration
                else if (previousMoveMagnitude - _internalMove.magnitude > decelerationMagnitude)
                {
                    _internalMove = _internalMove.normalized * (previousMoveMagnitude - decelerationMagnitude);
                }

                // cap max speed
                if (_internalMove.magnitude > maxSpeed)
                {
                    _internalMove = _internalMove.normalized * maxSpeed;
                }

                // animation stuff
                if (_animator != null)
                {
                    _animator.SetBool(_animatorBoolRunning, true);
                    _animator.speed = _internalMove.magnitude / maxSpeed;
                }
            }
            // no input deceleration
            else if (_internalMove.magnitude > decelerationMagnitude)
            {
                _internalMove -= _internalMove.normalized * decelerationMagnitude;

                // animation stuff
                if (_animator != null)
                {
                    _animator.SetBool(_animatorBoolRunning, false);
                    _animator.speed = 1;
                }
            }
            else
            {
                _internalMove = Vector3.zero;

                // animation stuff
                if (_animator != null)
                {
                    _animator.SetBool(_animatorBoolRunning, false);
                    _animator.speed = 1;
                }
            }

            // gravity is weird, have to multiply with deltatime twice
            Vector3 gravityDelta = Physics.gravity * Time.deltaTime * Time.deltaTime;

            // check grounded
            CheckGrounded();

            // reset or apply gravity
            if ((_controller.isGrounded && !_onSlope) || _resetGravity)
            {
                // character controller isn't grounded if it doesn't hit the ground every move method call
                _currentGravity = gravityDelta;
                _resetGravity = false;
            }
            else
            {
                _currentGravity += gravityDelta;
            }

            // make character controller move with all combined moves
            _controller.Move(_externalMove + _internalMove + (_onSlope ? _slopeDirection * _currentGravity.magnitude : _currentGravity));

            // reset external movement
            _externalMove = Vector3.zero;
        }

        if (!IsGrounded)
        {
            _airBorne = true;
            _animator?.SetBool(_animatorBoolAirborne, true);
        }
        else if (_airBorne && IsGrounded)
        {
            _airBorne = false;
            _animator?.SetBool(_animatorBoolAirborne, false);
            _landingSound?.PlaySound();
        }

        if (transform.position.y <= _minYPosition)
        {
            OutOfBounds();
        }
    }

    /// <summary>
    /// Return a vector with a magnitude of [0,1]f to move.
    /// </summary>
    /// <returns></returns>
    protected abstract Vector3 InternalMovement();

    /// <summary>
    /// Add a movement vector for character controller's move in the next FixedUpdate.
    /// </summary>
    /// <param name="move">exact movement vector, add deltaTime when necessary</param>
    public void AddDirectMovement(Vector3 move)
    {
        _externalMove += move;
    }

    /// <summary>
    /// Resets the current effect of gravity akin to hitting the ground.
    /// </summary>
    public void ResetGravity()
    {
        _resetGravity = true;
        _currentGravity = Vector3.zero;
    }

    /// <summary>
    /// Activates/deactivates character controller.
    /// </summary>
    public void SetControllerActive(bool active)
    {
        if (active && !_controllerEnabled)
        {
            _internalMove = Vector3.zero;
            _externalMove = Vector3.zero;
            ResetGravity();
            CheckGrounded();
        }

        _controller.enabled = active;
        _controllerEnabled = active;
    }

    private void CheckGrounded()
    {
        Vector3 upVector = -Physics.gravity.normalized;
        RaycastHit hit;
        if (Physics.SphereCast(
                transform.position + _controller.center,
                _controller.radius + _controller.skinWidth,
                Physics.gravity.normalized,
                out hit,
                (_controller.height / 2.0f) - _controller.radius + _groundedDistanceBonus,
                _walkableTerrain))
        {
            float slopeAngle = Vector3.Angle(upVector, hit.normal);
            _onSlope = slopeAngle > _controller.slopeLimit && slopeAngle < 90f;
            if (_onSlope)
            {
                _slopeDirection = Vector3.Cross(Vector3.Cross(upVector, hit.normal), hit.normal).normalized * slopeAngle / 90f;
            }
            IsGrounded = !_onSlope;
        }
        else
        {
            _onSlope = false;
            IsGrounded = false;
        }
    }

    protected abstract void OutOfBounds();
}