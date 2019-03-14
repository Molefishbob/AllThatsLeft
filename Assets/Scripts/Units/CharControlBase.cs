using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharControlBase : MonoBehaviour, IPauseable
{
    [Tooltip("Meters per second")]
    public float _speed = 6;
    [Tooltip("Degrees per second")]
    public float _turningSpeed = 540;
    [Tooltip("NOT in seconds")]
    public float _accelerationTime = 10;
    [SerializeField, Tooltip("The y position on which the unit dies")]
    private float _minYPosition = -10;
    public LayerMask _walkableTerrain = (1 << 12) + (1 << 13);
    public float _groundedDistance = 0.1f;

    protected CharacterController _controller;
    protected bool _paused;

    private Vector3 _externalMove = Vector3.zero;
    private Vector3 _internalMove = Vector3.zero;
    private Vector3 _currentGravity = Vector3.zero;
    private Vector3 _slopeDirection = Vector3.down;
    private bool _onSlope = false;
    private bool _resetGravity = false;
    private bool _controllerEnabled = true;
    private IDamageReceiver _damageReceiver = null;

    public bool IsGrounded { get; private set; }
    public float SkinWidth { get { return _controller.skinWidth; } }
    public float Radius { get { return _controller.radius; } }
    public float Height { get { return _controller.height; } }
    public float CurrentGravity { get { return _currentGravity.magnitude; } }

    public virtual void Pause()
    {
        _paused = true;
    }

    public virtual void UnPause()
    {
        _paused = false;
    }

    protected virtual void Awake()
    {
        _damageReceiver = GetComponent<IDamageReceiver>();
        _controller = GetComponent<CharacterController>();
    }

    protected virtual void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    protected virtual void OnDestroy()
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
            if (transform.position.y <= _minYPosition)
            {
                _damageReceiver.Die();
            }

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
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, inputRotation, _turningSpeed * Time.deltaTime);

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
                }
                // no input deceleration
                else if (_internalMove.magnitude > decelerationMagnitude)
                {
                    _internalMove -= _internalMove.normalized * decelerationMagnitude;
                }
                else
                {
                    _internalMove = Vector3.zero;
                }

                // gravity is weird, have to multiply with deltatime twice
                Vector3 gravityDelta = Physics.gravity * Time.deltaTime * Time.deltaTime;

                // check grounded
                Vector3 upVector = -Physics.gravity.normalized;
                RaycastHit hit;
                if (Physics.SphereCast(
                        transform.position + upVector * _controller.radius,
                        _controller.radius,
                        Physics.gravity.normalized,
                        out hit,
                        _groundedDistance,
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

            FixedUpdateAdditions();
        }
    }

    protected virtual void FixedUpdateAdditions() { }

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
    }

    /// <summary>
    /// Activates/deactivates character controller.
    /// </summary>
    public void SetControllerActive(bool active)
    {
        _controller.enabled = active;
        _controllerEnabled = active;

        if (active)
        {
            _internalMove = Vector3.zero;
            _externalMove = Vector3.zero;
            ResetGravity();
        }
    }
}