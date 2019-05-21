using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicLine : MonoBehaviour
{
    [SerializeField]
    private DeployControlledBots _deploy = null;
    [SerializeField]
    private LineRenderer _lr = null;
    [SerializeField]
    private float _maxDistance = 5.0f;
    [SerializeField]
    private float _distanceBetweenPoints = 0.1f;
    [SerializeField]
    private LayerMask _botCollisions = 1 << 12 | 1 << 13 | 1 << 14;
    [SerializeField]
    private float _minYPosition = -10.0f;
    [SerializeField]
    private LineRenderer _circle = null;

    private BotMovement _bot = null;

    private void LateUpdate()
    {
        if (_deploy == null || _lr == null) return;

        if (_bot == null && GameManager.Instance.BotPool != null)
        {
            _bot = GameManager.Instance.BotPool.transform.GetChild(0).GetComponent<BotMovement>();
        }

        float v = _deploy._throwDistance / _deploy._throwTime;
        float h = _deploy._throwHeight;
        float g = Physics.gravity.magnitude;
        float f = Mathf.Sqrt(2.0f * h * g);
        int totalPoints = (int)(_maxDistance / _distanceBetweenPoints) + 1;

        Vector3[] positions = new Vector3[totalPoints];

        int points = totalPoints;

        for (int i = 0; i < totalPoints; i++)
        {
            positions[i].z = i * _distanceBetweenPoints;
            float t = positions[i].z / v;
            positions[i].y = f * t - g * t * t / 2.0f;
            positions[i].x = 0.0f;

            if (i == 0) continue;

            if (_bot != null)
            {
                float radius = _bot._controller.skinWidth + _bot._controller.radius;
                RaycastHit[] hits = Physics.SphereCastAll(
                        transform.TransformPoint(positions[i - 1]) + Vector3.up * radius,
                        _bot._controller.radius,
                        transform.TransformDirection(positions[i] - positions[i - 1]),
                        Vector3.Distance(positions[i], positions[i - 1]),
                        _botCollisions);
                if (hits != null && hits.Length > 0)
                {
                    positions[i] = positions[i - 1];
                    RaycastHit[] hits2 = Physics.SphereCastAll(
                            transform.TransformPoint(positions[i]) + Vector3.up * radius,
                            _bot._controller.radius * 0.9f,
                            Vector3.down,
                            transform.position.y + positions[i].y - _minYPosition,
                            _botCollisions);
                    if (hits2 != null && hits2.Length > 0)
                    {
                        float highest = hits2[0].point.y;
                        foreach (RaycastHit hit in hits2)
                        {
                            if (hit.point.y > highest) highest = hit.point.y;
                        }
                        positions[i].y = highest;
                    }
                    else
                    {
                        positions[i].y = _minYPosition;
                    }
                    positions[i].y -= transform.position.y;
                    points = i + 1;
                    break;
                }
                else if (transform.position.y + positions[i].y <= _minYPosition)
                {
                    points = i + 1;
                    break;
                }
            }
        }

        _lr.positionCount = points;
        _lr.SetPositions(positions);

        _circle.transform.position = transform.TransformPoint(_lr.GetPosition(_lr.positionCount - 1)) + Vector3.up * _circle.startWidth;
    }
}
