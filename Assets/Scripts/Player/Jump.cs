using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private bool _canJump;
    private CharacterController _controller;
    public float _jumpForce;
    public float _gravity;
    private Vector3 _jump = Vector3.zero;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        
    }
   
    private void Update()
    {

        if (_canJump)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _jump.y = _jumpForce;
                _canJump = false;
            }
        }

        _jump.y = _jump.y - (_gravity * Time.deltaTime);

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
