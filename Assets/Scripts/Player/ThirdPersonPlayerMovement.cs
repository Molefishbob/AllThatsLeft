using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerMovement : MonoBehaviour, IPauseable
{
    [Tooltip("Meters per second")]
    public float _speed;
    [Tooltip("Degrees per second")]
    public float _turningSpeed;
    [Tooltip("In seconds")]
    public float _accelerationTime;
    [Tooltip("In seconds")]
    public float _decelerationTime;

    [HideInInspector]
    public Vector3 _externalMove = Vector3.zero;
    [HideInInspector]
    public CharacterController _controller;

    private float _previousDesiredSpeed = 0.0f;
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
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            // create combined vector of input
            Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

            // get desired speed from vector magnitude
            float desiredSpeed = Mathf.Clamp(inputDirection.magnitude, 0.0f, 1.0f);

            if (desiredSpeed > 0.0f && desiredSpeed >= _previousDesiredSpeed)
            {
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

                // move character only directly forward
                //_currentMove += transform.forward * _speed * Time.deltaTime / _accelerationTime;

                // move directly in input direction ignoring rotation
                _internalMove += inputDirection * _speed * Time.deltaTime / _accelerationTime;

                // drop extra speed
                if (_internalMove.magnitude > desiredSpeed)
                {
                    _internalMove = _internalMove.normalized * desiredSpeed;
                }
            }
            else if (_internalMove.magnitude > 0.0f)
            {
                Vector3 drag = -_internalMove.normalized * _speed * Time.deltaTime / _decelerationTime;
                if (_internalMove.magnitude > drag.magnitude)
                {
                    _internalMove += drag;
                }
                else
                {
                    _internalMove = Vector3.zero;
                }
            }

            _previousDesiredSpeed = desiredSpeed;

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
