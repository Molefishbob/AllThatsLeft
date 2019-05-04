using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLine : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lr = null;
    private GenericMover _platform = null;
    [SerializeField]
    private float _offset = 1f;

    private void Start()
    {
        _platform = GetComponent<GenericMover>();
        
        List<Transform> pointList = _platform._transform;

        int totalPoints;

        if (_platform.GetComponent<MoveBetweenLoop>() != null)
        {
            totalPoints = pointList.Count + 1;
            pointList.Add(pointList[0]);
        } else
        {
            totalPoints = pointList.Count;
        }

        Vector3[] positions = new Vector3[totalPoints];

        int points = totalPoints;

        for (int i = 0; i < totalPoints; i++)
        {
            positions[i].z = pointList[i].transform.position.z;
            positions[i].y = pointList[i].transform.position.y - _offset;
            positions[i].x = pointList[i].transform.position.x;

        }

        _lr.positionCount = points;
        _lr.SetPositions(positions);

    }
}
