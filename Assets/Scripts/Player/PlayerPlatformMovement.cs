using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformMovement : MonoBehaviour, IPauseable
{
    public float _linearDrag;
    public LayerMask _platformLayerMask;

    private Transform _platform;
    private Vector3 _lastPlatformPos;
    private Vector3 _currentPlatformMove;
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
            RaycastHit hit;
            if (_player._controller.isGrounded)
            {
                if (Physics.SphereCast(
                        transform.position + Vector3.up * _player._controller.radius,
                        _player._controller.radius,
                        Physics.gravity,
                        out hit,
                        _player._controller.skinWidth * 1.1f,
                        _platformLayerMask))
                {
                    if (_platform == null)
                    {
                        _platform = hit.transform;
                    }
                    else
                    {
                        _currentPlatformMove = _platform.position - _lastPlatformPos;
                        _player._externalMove += _currentPlatformMove;
                    }

                    _lastPlatformPos = _platform.position;
                }
                else
                {
                    _platform = null;
                    _currentPlatformMove = Vector3.zero;
                }
            }
            else
            {
                _platform = null;

                Vector3 drag = -_currentPlatformMove.normalized * _linearDrag * Time.deltaTime;

                if (_currentPlatformMove.magnitude > drag.magnitude)
                {
                    _currentPlatformMove += drag;
                    _player._externalMove += _currentPlatformMove;
                }
                else
                {
                    _currentPlatformMove = Vector3.zero;
                }
            }
        }
    }
}