using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlPlatformMovement : MonoBehaviour
{
    [SerializeField]
    private LayerMask _platformLayerMask = 1 << 13;
    [SerializeField]
    private float _disconnectDistance = 2.5f;

    [HideInInspector]
    public GenericMover _platform;
    private CharControlBase _character;
    private bool _forced = false;

    public Vector3 CurrentMove { get; private set; }

    private void Awake()
    {
        _character = GetComponent<CharControlBase>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        Vector3 pos = _character.transform.position;

        if (_platform == null)
        {
            _platform = FindPlatform(0);
            if (_platform != null) pos.y = _platform.transform.position.y;
        }
        else
        {
            GenericMover plat = FindPlatform(_forced ? Mathf.Infinity : _disconnectDistance);
            if (plat != _platform && plat != null) pos.y = _platform.transform.position.y;
            _platform = plat;
        }

        if (_platform != null)
        {
            pos.y += _platform.CurrentMove.y;
            _character.transform.position = pos;
            Vector3 move = _platform.CurrentMove;
            move.y = 0.0f;
            _character.AddDirectMovement(move);
            CurrentMove = _platform.CurrentMove;
        }
        else
        {
            CurrentMove = Vector3.zero;
            _forced = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if ((hit.controller.collisionFlags & CollisionFlags.Below) != 0)
        {
            _platform = null;
            _forced = false;
        }

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

    public void ForcePlatform(GenericMover platform)
    {
        _platform = platform;
        _forced = true;
    }
}
