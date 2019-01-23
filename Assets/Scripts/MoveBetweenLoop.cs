using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenLoop: GenericMover
{
    


    // FixedUpdate is called once per physics update
    void FixedUpdate()
    {
        _fracTime = (Time.time - _eventTime) / (_duration *
                (_transform[_currentObjectNum].position - _transform[_nextObjectNum].position).magnitude / _length);
        
        transform.position = 
                            Vector3.Lerp(_transform[_currentObjectNum].position
                                        , _transform[_nextObjectNum].position, _fracTime);

        if (_fracTime >= 1)
        {
            ChangeTarget();
        }
    }

    /// <summary>
    /// Changes the direction the object is travelling
    /// </summary>
    private void ChangeTarget()
    {
        
        _currentObjectNum = _nextObjectNum;
        _nextObjectNum = _currentObjectNum + 1;

        if (_nextObjectNum > _amountOfTransforms)
        {
            _nextObjectNum = 0;
            _eventTime = Time.time;
        } else if (_currentObjectNum == 0)
        {
            _trackRecord++;
            _eventTime = _ogStartTime + _trackRecord * _duration;
        } else
        {
            _eventTime = Time.time;
        }
    }

    public override void Init()
    {
        base.Init();
        _length += (_transform[0].position - _transform[_amountOfTransforms].position).magnitude;
    }
}
