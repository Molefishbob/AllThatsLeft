using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlPlatformMovement : MonoBehaviour
{
    public LayerMask _platformLayerMask = 1 << 13;
    public float _disconnectDistance = 2.5f;

    private GenericMover _platform;
    private CharControlBase _character;

    private void Awake()
    {
        _character = GetComponent<CharControlBase>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        RaycastHit hit2;
        if (Physics.SphereCast(
                transform.position + _character._controller.center,
                _character._controller.radius + _character._controller.skinWidth,
                Physics.gravity.normalized,
                out hit2,
                (_character._controller.height / 2.0f) - _character._controller.radius - _character._controller.skinWidth,
                _platformLayerMask))
        {
            if (_platform == null) _platform = hit2.transform.GetComponent<GenericMover>();
            if (_platform.CurrentMove.y > 0.0f)
            {
                _character.NoGravity();
            }
        }

        if (_platform == null) return;

        RaycastHit hit;
        if (Physics.SphereCast(
                transform.position + _character._controller.center,
                _character._controller.radius + _character._controller.skinWidth,
                Physics.gravity.normalized,
                out hit,
                (_character._controller.height / 2.0f) - _character._controller.radius + _disconnectDistance,
                _platformLayerMask))
        {
            if (_platform != hit.transform.GetComponent<GenericMover>())
            {
                _platform = null;
            }
            else
            {
                _character.AddDirectMovement(_platform.CurrentMove);
            }
        }
        else
        {
            _platform = null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GenericMover gm = hit.gameObject.GetComponent<GenericMover>();
        if (gm != null) _platform = gm;
    }
}