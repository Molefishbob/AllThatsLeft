using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlPlatformMovement : MonoBehaviour
{
    [SerializeField]
    private LayerMask _platformLayerMask = 1 << 13;
    [SerializeField]
    private float _disconnectDistance = 2.5f;

    private GenericMover _platform;
    private CharControlBase _character;

    private void Awake()
    {
        _character = GetComponent<CharControlBase>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_platform == null)
        {
            _platform = FindPlatform(0);
            if (_platform != null)
            {
                Vector3 pos = _character.transform.position + _platform.CurrentMove;
                pos.y = _platform.transform.position.y;
                _character.transform.position = pos;
            }
        }
        else if (FindPlatform(_disconnectDistance) != _platform)
        {
            _platform = null;
        }
        else
        {
            _character.transform.position += _platform.CurrentMove;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GenericMover gm = hit.gameObject.GetComponent<GenericMover>();
        if (gm != null) _platform = gm;
    }

    private GenericMover FindPlatform(float distance)
    {
        RaycastHit hit;
        if (Physics.SphereCast(
                transform.position + _character._controller.center,
                _character._controller.radius + _character._controller.skinWidth,
                Physics.gravity.normalized,
                out hit,
                (_character._controller.height / 2.0f) - _character._controller.radius + distance,
                _platformLayerMask))
        {
            return hit.transform.GetComponent<GenericMover>();
        }
        return null;
    }
}