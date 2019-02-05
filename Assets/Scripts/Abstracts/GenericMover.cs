using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericMover : MonoBehaviour
{

    [Tooltip("The amount of time it takes to go the whole length")]
    public float _duration;
    protected float _eventTime;
    protected List<Transform> _transform;
    protected float _fracTime;
    protected int _amountOfTransforms;
    protected int _currentObjectNum = 0;
    protected int _nextObjectNum = 1;
    protected float _length;
    protected float _ogStartTime;
    protected int _trackRecord = 0;
    protected bool _activated;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        _eventTime = Time.time;
        _ogStartTime = _eventTime;

        _transform = new List<Transform>(transform.parent.childCount);

        foreach (Transform child in transform.parent) {
            if (child != transform) {
                _transform.Add(child);
            }
        }
        
        _amountOfTransforms = _transform.Count - 1;

        for (int a = 0; a < _transform.Count-1; a++)
        {
            _length += (_transform[a].position - _transform[a + 1].position).magnitude;
        }
    }
}
