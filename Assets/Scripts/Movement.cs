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
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 forward = vertical * _speed * Vector3.forward;

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
