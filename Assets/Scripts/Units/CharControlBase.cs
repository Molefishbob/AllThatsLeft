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
    [SerializeField, Tooltip("NOT in seconds")]
    private float _decelerationTime = 5;
    [SerializeField, Tooltip("The y position on which the unit dies")]
    private float _minYPosition = -10;
    [SerializeField]
    private float _minAirborneTime = 0.2f;
    [SerializeField]
    protected LayerMask _walkableTerrain = 1 << 12 | 1 << 13 | 1 << 14;
    [SerializeField]
    private RandomSFXSound _landingSound = null;
    [SerializeField]
    private string _animatorBoolRunning = "Run";
    public string _animatorBoolAirborne = "Airborne";
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
    private bool _holdPosition = false;
    private PhysicsOneShotTimer _airTimer;

    public bool HoldPosition
    {
        get
        {
            return _holdPosition;
        }
        set
        {
            _holdPosition = value;
            ResetInternalMove();
        }
    }

    public bool IsGrounded { get; private set; }
    public float CurrentGravity { get { return _currentGravity.magnitude; } }

    protected virtual void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>(true);
        _airTimer = gameObject.AddComponent<PhysicsOneShotTimer>();
        SetControllerActive(_startsActive);
    }

    protected virtual void Start()
    {
        _airTimer.OnTimerCompleted += Aired;
    }

    protected virtual void OnDestroy()
    {
        _airTimer.OnTimerCompleted -= Aired;
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
                decelerationMagnitude = maxSpeed / _decelerationTime;
            }
            else
            {
                accelerationMagnitude = _speed;
                decelerationMagnitude = _speed;
            }

            // get movement from child class
            Vector2 inputMove = InternalMovement();
            Vector3 inputDirection = new Vector3(inputMove.x, 0.0f, inputMove.y);

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

                if (HoldPosition)
                {
                    _animator.SetBool(_animatorBoolRunning, false);
                }
                else
                {
                    // decelerate
                    Vector3 decelerationVector = inputDirection * maxSpeed - _internalMove;
                    decelerationVector = decelerationVector.normalized * Mathf.Min(decelerationVector.magnitude, decelerationMagnitude);
                    _internalMove += decelerationVector;

                    // add to current movement
                    _internalMove += inputDirection * accelerationMagnitude;

                    // cap max speed
                    _internalMove = _internalMove.normalized * Mathf.Min(_internalMove.magnitude, maxSpeed);

                    _animator.SetBool(_animatorBoolRunning, true);
                }
            }
            // no input deceleration
            else
            {
                _internalMove = _internalMove.normalized * Mathf.Max(_internalMove.magnitude - decelerationMagnitude, 0.0f);

                _animator.SetBool(_animatorBoolRunning, false);
            }

            // gravity is weird, have to multiply with deltatime twice
            Vector3 gravityDelta = Physics.gravity * Time.deltaTime * Time.deltaTime;

            // reset or apply gravity
            if (IsGrounded || _resetGravity)
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

            // check grounded
            IsGrounded = _controller.isGrounded && !_onSlope;

            // reset external movement
            _externalMove = Vector3.zero;

            if (_controller.collisionFlags == CollisionFlags.None)
            {
                _onSlope = false;
            }
        }

        if (!IsGrounded && !_airBorne && !_airTimer.IsRunning)
        {
            _airTimer.StartTimer(_minAirborneTime);
        }
        else if (IsGrounded)
        {
            _airTimer.StopTimer();
            _animator.SetBool(_animatorBoolAirborne, false);
            if (_airBorne)
            {
                _airBorne = false;
                if (_landingSound != null) _landingSound.PlaySound();
            }
        }

        if (transform.position.y <= _minYPosition)
        {
            OutOfBounds();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
        if (slopeAngle <= _controller.slopeLimit)
        {
            _onSlope = false;
            return;
        }
        if (slopeAngle > 90.0f) return;

        _onSlope = true;
        _slopeDirection = Vector3.Cross(Vector3.Cross(Vector3.up, hit.normal), hit.normal).normalized * slopeAngle / 90f;
    }

    private void Aired()
    {
        _airBorne = true;
        _animator.SetBool(_animatorBoolAirborne, true);
    }

    /// <summary>
    /// Return a vector with a magnitude of [0,1]f to move in X and Z axes.
    /// </summary>
    /// <returns></returns>
    protected abstract Vector2 InternalMovement();

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
        }

        _controller.enabled = active;
        _controllerEnabled = active;
        // if (_animator != null) _animator.speed = 1;
    }

    /// <summary>
    /// Resets internal movement which stops character controller instantly on the next frame.
    /// </summary>
    public void ResetInternalMove()
    {
        _internalMove = Vector3.zero;
    }

    protected abstract void OutOfBounds();

    /// <summary>
    /// Rotates a Vector2 by an angle.
    /// </summary>
    /// <param name="v">Vector2 to rotate</param>
    /// <param name="angle">angle in degrees</param>
    /// <returns></returns>
    public static Vector2 RotateInput(Vector2 v, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        Vector2 result = new Vector2(cos * v.x + sin * v.y, cos * v.y - sin * v.x);
        return result;
    }
}
