using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlPlatformMovement : MonoBehaviour
{
    public LayerMask _platformLayerMask = 1 << 13;
    public float _attachDistance = 0.2f;
    public float _disconnectDistance = 2.5f;

    private Transform _platform;
    private Vector3 _lastPlatformPos;
    private CharControlBase _character;
    private float _currentAttachDistance;

    private void Awake()
    {
        _character = GetComponent<CharControlBase>();
        _currentAttachDistance = _attachDistance;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        RaycastHit hit;

        if (Physics.SphereCast(
                transform.position + _character._controller.center,
                _character._controller.radius + _character._controller.skinWidth,
                Physics.gravity.normalized,
                out hit,
                (_character._controller.height / 2.0f) + _character._controller.skinWidth + _currentAttachDistance,
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