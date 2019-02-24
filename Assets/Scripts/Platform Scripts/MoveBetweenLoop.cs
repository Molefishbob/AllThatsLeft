﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenLoop: GenericMover
{
    


    // FixedUpdate is called once per physics update
    void FixedUpdate()
    {
        if (_activated)
        {
            float currLength = (_timer.TimeElapsed) / (_timer.Duration) * _length;

            for (int i = 0; i < _currentObjectNum; ++i)
            {
                currLength -= (_transform[i].position - _transform[i + 1].position).magnitude;
            }

            _fracTime = currLength / (_transform[_currentObjectNum].position - _transform[_nextObjectNum].position).magnitude;

            transform.position =
                                Vector3.Lerp(_transform[_currentObjectNum].position
                                            , _transform[_nextObjectNum].position, _fracTime);

            if (_fracTime >= 1)
            {
                ChangeTarget();
            }
        }
    }

    /// <summary>
    /// Changes the direction the object is travelling
    /// </summary>
    private void ChangeTarget()
    {
        _currentObjectNum = _nextObjectNum;
        _nextObjectNum = _currentObjectNum + 1;
        if (_nextObjectNum >= _transform.Count)
        {
            _nextObjectNum = 0;
        }
    }
    /// <summary>
    /// Initializes the script.
    /// 
    /// Adds the distance between the last and the first object to the complete length.
    /// </summary>
    public override void Init()
    {
        base.Init();
        _length += (_transform[0].position - _transform[_amountOfTransforms].position).magnitude;
    }
    /// <summary>
    /// Called when the timer is completed.
    /// 
    /// Defines what happens when the timer has completed.
    /// </summary>
    public override void TimedAction()
    {
        _nextObjectNum = 1;
        _currentObjectNum = 0;
    }
}