using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour, IPauseable
{
    public float _jumpForce;

    private ThirdPersonPlayerMovement _player;
    private bool _jumping;

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
            if (_player.IsGrounded)
            {
                _jumping = Input.GetButton("Jump");
            }

            if (_jumping)
            {
                Debug.Log("JUMPING");
                _player._currentMove += Vector3.up * _jumpForce * Time.deltaTime;
            }
        }
    }
}