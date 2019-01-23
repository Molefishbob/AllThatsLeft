using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericMover : MonoBehaviour
{

    [Tooltip("Enter all the transforms here.\nWorks with any number of transforms.")]
    public Transform[] _transform;
    [Tooltip("The amount of time it takes to go the whole length")]
    public float _duration;
    protected float _eventTime;
    protected float _fracTime;
    protected int _amountOfTransforms;
    protected int _currentObjectNum = 0;
    protected int _nextObjectNum = 1;
    protected float _length;
    protected float _ogStartTime;
    protected int _trackRecord = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        _eventTime = Time.time;
        _ogStartTime = _eventTime;
        _amountOfTransforms = _transform.Length - 1;

        for (int a = 0; a < _transform.Length-1; a++)
        {
            _length += (_transform[a].position - _transform[a + 1].position).magnitude;
        }
    }
}
