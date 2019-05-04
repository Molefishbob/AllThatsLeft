using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowLimiter : MonoBehaviour
{
    [SerializeField]
    private float _extraDistance = 0.2f;
    [SerializeField]
    private float _maxShadowDistance = 50.0f;
    [SerializeField]
    private float _radiusMultiplier = 0.5f;
    [SerializeField]
    private LayerMask _shadowableTerrain = 1 << 12 | 1 << 13 | 1 << 14;

    private Projector _projector;

    private void Awake()
    {
        _projector = GetComponent<Projector>();
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        float radius = _projector.orthographicSize * _radiusMultiplier;
        RaycastHit hit;
        if (Physics.SphereCast(
                transform.position + Vector3.up * (radius + _extraDistance),
                radius,
                Vector3.down,
                out hit,
                _maxShadowDistance,
                _shadowableTerrain))
        {
            _projector.farClipPlane = hit.distance;
        }
        else
        {
            _projector.farClipPlane = _maxShadowDistance + radius + _extraDistance;
        }
    }
}
