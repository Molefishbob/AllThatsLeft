using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleLine : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lr = null;
    [SerializeField]
    private float _radius = 0.5f;
    [SerializeField]
    private int _totalPoints = 64;

    private void LateUpdate()
    {
        if (_lr == null) return;

        Vector3[] positions = new Vector3[_totalPoints];

        for (int i = 0; i < _totalPoints; i++)
        {
            positions[i].x = Mathf.Sin(2.0f * Mathf.PI * (float)i / (float)_totalPoints) * _radius;
            positions[i].z = Mathf.Cos(2.0f * Mathf.PI * (float)i / (float)_totalPoints) * _radius;
            positions[i].y = 0.0f;
        }

        _lr.positionCount = _totalPoints;
        _lr.SetPositions(positions);
    }
}
