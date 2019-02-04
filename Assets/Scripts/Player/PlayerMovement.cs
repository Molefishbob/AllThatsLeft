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
    private Transform _trans;
    private bool _canJump;
    public float _jumpForce;
    private Vector3 _jump = Vector3.zero;

    public static float Yaw
    {
        get { return _yaw; }
    }

    private void Awake()
    {
        _trans = GetComponent<Transform>();
        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<CameraScript>();
    }

    void FixedUpdate()
    {

        int layerMask = 1 << 10;
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.5f, new Vector3(0, -0.5f, 0), out hit, 2.5f, layerMask))
        {
            // Makes player the child of the platform he is standing on
            gameObject.transform.parent = hit.transform;
        } else
        {
            gameObject.transform.parent = null;
        }

        _yaw += _horizontalSensitivity * Input.GetAxis("Mouse X");
        transform.eulerAngles = new Vector3(0, _yaw, 0);

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 forward = vertical * _speed * Vector3.forward;

        if (vertical < 0)
        {
            forward *= _backwardMultiplier;
            vertical *= _backwardMultiplier;
        }

        //_trans.position += transform.forward * _speed * vertical * Time.deltaTime;

        Vector3 sideWays = _strafeSpeed * horizontal * Vector3.right;
        //_trans.position += transform.right * _strafeSpeed * horizontal * Time.deltaTime;

        Vector3 movement = (forward + sideWays) * Time.deltaTime;
        
        movement = transform.TransformVector(movement);
      

        _controller.Move(movement);

        if (_canJump)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _jump.y = _jumpForce;
                _canJump = false;
            }
        }

        _jump += (Physics.gravity * Time.deltaTime);

        _controller.Move(_jump * Time.deltaTime);
    }

    public void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.gameObject.layer == 9 || col.gameObject.layer == 10)
        {
            _canJump = true;
        }
    }
}
