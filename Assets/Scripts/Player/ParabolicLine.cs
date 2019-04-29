using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParabolicLine : MonoBehaviour
{
    [SerializeField]
    private DeployControlledBots _deploy = null;
    [SerializeField]
    private LineRenderer _lr = null;
    [SerializeField]
    private float _maxDistance = 5.0f;
    [SerializeField]
    private int _totalPoints = 501;

    private void Update()
    {
        if (_deploy == null || _lr == null) return;

        float v = _deploy._throwDistance / _deploy._throwTime;
        float h = _deploy._throwHeight;
        float g = Physics.gravity.magnitude;
        float f = Mathf.Sqrt(2.0f * h * g);

        Vector3[] positions = new Vector3[_totalPoints];

        for (int i = 0; i < _totalPoints; i++)
        {
            positions[i].z = i * _maxDistance / (_totalPoints - 1);
            float t = positions[i].z / v;
            positions[i].y = f * t - g * t * t / 2.0f;
            positions[i].x = 0.0f;
        }

        _lr.positionCount = _totalPoints;
        _lr.SetPositions(positions);
    }
}
