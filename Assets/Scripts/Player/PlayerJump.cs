using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour, IPauseable
{
    public float _jumpHeight;

    private Vector3 _jumpForce;
    private bool _jumping;
    private ThirdPersonPlayerMovement _player;

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
        _player = GetComponent<ThirdPersonPlayerMovement>();
        _jumpForce = -Physics.gravity.normalized * Mathf.Sqrt(2f * _jumpHeight * Physics.gravity.magnitude);
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
            if (_player._controller.isGrounded)
            {
                _jumping = Input.GetButton("Jump");
            }

            if (_jumping)
            {
                _player._externalMove += _jumpForce * Time.deltaTime;
            }
        }
    }
}