using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerMovement : MonoBehaviour, IPauseable
{
    [Tooltip("Meters per second")]
    public float _speed;
    [Tooltip("Degrees per second")]
    public float _turningSpeed;
    [Tooltip("NOT in seconds")]
    public float _accelerationTime;
    [Tooltip("NOT in seconds")]
    public float _decelerationTime;
    public string _horizontalAxis = "Horizontal";
    public string _verticalAxis = "Vertical";

    [HideInInspector]
    public Vector3 _externalMove = Vector3.zero;
    [HideInInspector]
    public CharacterController _controller;

    private Vector3 _internalMove = Vector3.zero;
    private Vector3 _currentGravity = Vector3.zero;
    private Transform _cameraTransform;
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
        _controller = GetComponent<CharacterController>();
        _cameraTransform = FindObjectOfType<Camera>().transform;
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
            // read input
            float horizontal = Input.GetAxisRaw(_horizontalAxis);
            float vertical = Input.GetAxisRaw(_verticalAxis);

            float maxSpeed = _speed * Time.deltaTime;
            float accelerationMagnitude;
            float decelerationMagnitude;

            if (_accelerationTime > 0)
            {
                accelerationMagnitude = maxSpeed / _accelerationTime;
            }
            else
            {
                accelerationMagnitude = _speed;
            }

            if (_decelerationTime > 0)
            {
                decelerationMagnitude = maxSpeed / _decelerationTime;
            }
            else
            {
                decelerationMagnitude = _speed;
            }

            if (horizontal != 0.0f || vertical != 0.0f)
            {
                // create combined vector of input
                Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

                // get desired speed from vector magnitude
                float desiredSpeed = Mathf.Clamp(inputDirection.magnitude, 0.0f, 1.0f);

                // convert to world space relative to camera
                inputDirection = _cameraTransform.TransformDirection(inputDirection);

                // remove pitch
                inputDirection.y = 0;

                // normalize direction
                inputDirection.Normalize();

                // convert input direction to a rotation
                Quaternion inputRotation = Quaternion.LookRotation(inputDirection, Vector3.up);

                // rotate character towards input rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, inputRotation, _turningSpeed * Time.deltaTime);

                // save previous move's magnitude
                float previousMoveMagnitude = _internalMove.magnitude;

                // add to current movement
                _internalMove += inputDirection * desiredSpeed * accelerationMagnitude;

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
}
