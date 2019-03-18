using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlPlatformMovement : MonoBehaviour, IPauseable
{
    public LayerMask _platformLayerMask = 1 << 13;
    public float _attachDistance = 0.2f;
    public float _disconnectDistance = 2.5f;

    private Transform _platform;
    private Vector3 _lastPlatformPos;
    private CharControlBase _character;
    private float _currentAttachDistance;

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
        _currentAttachDistance = _attachDistance;
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

            if (Physics.SphereCast(
                    transform.position + _character.Center,
                    _character.Radius,
                    Physics.gravity.normalized,
                    out hit,
                    (_character.Height / 2.0f) + _character.SkinWidth + _currentAttachDistance,
                    _platformLayerMask))
            {
                if (_platform == null)
                {
                    _platform = hit.transform;
                    _currentAttachDistance = _disconnectDistance;
                }
                else if (_platform != hit.transform)
                {
                    _platform = null;
                    _currentAttachDistance = _attachDistance;
                }
                else
                {
                    Vector3 currentPlatformMove = _platform.position - _lastPlatformPos;
                    _character.AddDirectMovement(currentPlatformMove);
                }

                if (_platform != null)
                {
                    _lastPlatformPos = _platform.position;
                }
            }
            else
            {
                _platform = null;
                _currentAttachDistance = _attachDistance;
            }
        }
    }
}