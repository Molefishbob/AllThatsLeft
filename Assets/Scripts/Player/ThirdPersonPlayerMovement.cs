using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerMovement : MonoBehaviour, IPauseable
{
    [Tooltip("Meters per second")]
    public float _speed;
    [Tooltip("Degrees per second")]
    public float _turningSpeed;

    [HideInInspector]
    public Vector3 _currentMove = Vector3.zero;
    [HideInInspector]
    public bool IsGrounded { get { return _controller.isGrounded; } }

    private Vector3 _currentGravity = Vector3.zero;
    private CharacterController _controller;
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

            if (desiredSpeed > 0.0f)
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

                // add to current move
                _currentMove += transform.forward * desiredSpeed * _speed * Time.deltaTime;
            }

            // reset or apply gravity
            if (_controller.isGrounded)
            {
                _currentGravity = Vector3.zero;
            }
            else
            {
                // gravity is weird
                _currentGravity += Physics.gravity * Time.deltaTime * Time.deltaTime;
            }

            // make character controller move with all combined moves
            _controller.Move(_currentMove + _currentGravity);

            // reset movement
            _currentMove = Vector3.zero;
        }
    }
}
