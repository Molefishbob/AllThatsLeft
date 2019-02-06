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
    public float _horizontalSensitivity = 2.0f;
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

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<CameraScript>();
    }

    void FixedUpdate()
    {
        _yaw += _horizontalSensitivity * Input.GetAxis("Mouse X");
        transform.eulerAngles = new Vector3(0, _yaw, 0);

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

        int layerMask = 1 << 10;
        RaycastHit hit;
        Vector3 platformMoveDist = Vector3.zero;
     
        if (Physics.SphereCast(transform.position, 0.5f, new Vector3(0, -0.5f, 0), out hit, 2.5f, layerMask))
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

    public void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.gameObject.layer == 9 || col.gameObject.layer == 10)
        {
            _canJump = true;
        }
    }
}
