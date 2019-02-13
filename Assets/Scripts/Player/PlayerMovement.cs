using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float _speed;
    [Range(0,1)]
    public float _backwardMultiplier;
    public float _strafeSpeed;
    private CharacterController _controller;
    private float _mouseSensitivity = 2.0f;
    private static float _cameraSensitivity;
    private static float _yaw = 0.0f;
    private CameraScript _camera;
    private bool _canJump;
    public float _jumpForce;
    private Vector3 _jump = Vector3.zero;
    private Vector3 _platformPosition;
    private bool isOnPlatform;

    public static float Yaw
    {
        get { return _yaw; }
    }

    public static float Sensitivity
    {
        get { return _cameraSensitivity; }
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<CameraScript>();
        _cameraSensitivity = _mouseSensitivity;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        int platformLayerMask = 1 << 13;
        int groundLayerMask = 1 << 12;
        int jumpLayerMask = groundLayerMask | platformLayerMask;
        if (Physics.SphereCast(transform.position, _controller.radius * 0.9f, new Vector3(0, -1, 0), out hit, 0.9f, jumpLayerMask))
        {
            _canJump = true;
        }

        _yaw += Input.GetAxis("Mouse X");
        Quaternion rotation = Quaternion.Euler( 0, _yaw, 0);
        transform.rotation = rotation;
        //transform.eulerAngles = new Vector3(0, _yaw, 0);

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical < 0)
        {
            vertical *= _backwardMultiplier;
        }

        Vector3 forward = vertical * _speed * Vector3.forward;

        Vector3 sideWays = _strafeSpeed * horizontal * Vector3.right;

        if (_canJump)
        {
            if (Input.GetButton("Jump"))
            {
                _jump.y = _jumpForce;
                _canJump = false;
            }
        }

        _jump += (Physics.gravity * Time.deltaTime);
        
        Vector3 movement = (forward + sideWays + _jump) * Time.deltaTime;

        movement = transform.TransformDirection(movement);
        
        _controller.Move(movement);

        Vector3 platformMoveDist = Vector3.zero;
     
        if (Physics.SphereCast(transform.position, _controller.radius * 0.9f, new Vector3(0, -1, 0), out hit, 3, platformLayerMask))
        {
            Vector3 newPlatformPosition = hit.transform.position;
            platformMoveDist = newPlatformPosition - _platformPosition;
            if (_platformPosition != newPlatformPosition)
            {
                _platformPosition = newPlatformPosition;
            }
            if (isOnPlatform)
            {
                transform.position += platformMoveDist;
            }
            isOnPlatform = true;
        }
        else
        {
            platformMoveDist = Vector3.zero;
            isOnPlatform = false;
        } 
    }
}
