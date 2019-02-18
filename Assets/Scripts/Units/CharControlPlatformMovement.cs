using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlPlatformMovement : MonoBehaviour, IPauseable
{
    [Tooltip("Momentum decay speed")]
    public float _linearDrag = 0.01f;
    public LayerMask _platformLayerMask = 1 << 13;

    private Transform _platform;
    private Vector3 _lastPlatformPos;
    private Vector3 _currentPlatformMove;
    private CharControlBase _character;

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
        _character = GetComponent<CharControlBase>();
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
            if (_character.IsGrounded)
            {
                if (Physics.SphereCast(
                        transform.position + Vector3.up * _character.Radius,
                        _character.Radius,
                        Physics.gravity,
                        out hit,
                        _character.SkinWidth * 1.1f,
                        _platformLayerMask))
                {
                    if (_platform == null)
                    {
                        _platform = hit.transform;
                    }
                    else
                    {
                        _currentPlatformMove = _platform.position - _lastPlatformPos;
                        _character.AddDirectMovement(_currentPlatformMove);
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
                    _character.AddDirectMovement(_currentPlatformMove);
                }
                else
                {
                    _currentPlatformMove = Vector3.zero;
                }
            }
        }
    }
}