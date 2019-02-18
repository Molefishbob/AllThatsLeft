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

    protected CharacterController _controller;
    protected bool _paused;

    private Vector3 _externalMove = Vector3.zero;
    private Vector3 _internalMove = Vector3.zero;
    private Vector3 _currentGravity = Vector3.zero;

    public bool IsGrounded { get { return _controller.isGrounded; } }
    public float SkinWidth { get { return _controller.skinWidth; } }
    public float Radius { get { return _controller.radius; } }

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

            // reset or apply gravity
            if (_controller.isGrounded)
            {
                // character controller isn't grounded if it doesn't hit the ground every move method call
                _currentGravity = Physics.gravity * Time.deltaTime * Time.deltaTime;
            }
            else
            {
                // gravity is weird, have to multiply with deltatime twice
                _currentGravity += Physics.gravity * Time.deltaTime * Time.deltaTime;
            }

            // make character controller move with all combined moves
            _controller.Move(_externalMove + _internalMove + _currentGravity);

            // reset external movement
            _externalMove = Vector3.zero;
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
    /// <param name="move">Movement, remember deltaTime if applicable</param>
    public void AddDirectMovement(Vector3 move)
    {
        _externalMove += move;
    }
}