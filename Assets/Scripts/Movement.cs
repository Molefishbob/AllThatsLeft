using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float _speed;
    [Range(0,1)]
    public float _backwardMultiplier;
    public float _strafeSpeed;
    private CharacterController _controller;
    public float _horizontalSensitivity = 2.0f;
    private static float _yaw = 0.0f;
    private CameraScript _camera;

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

        int layerMask = 1 << 10;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.5f , layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            gameObject.transform.parent = hit.transform;
        } else
        {
            gameObject.transform.parent = null;
        }

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        _yaw += _horizontalSensitivity * Input.GetAxis("Mouse X");

        Vector3 forward = vertical * _speed * Vector3.forward;

        transform.eulerAngles = new Vector3(0, _yaw, 0);

        if (vertical  < 0)
        {
            forward *= _backwardMultiplier;
        }

        Vector3 sideWays = _strafeSpeed * Vector3.right * horizontal;

        Vector3 movement = (forward + sideWays) * Time.deltaTime;

        movement = transform.TransformVector(movement);

        _controller.Move(movement);
    }
}
