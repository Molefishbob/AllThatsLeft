using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenBackForth : GenericMover {

	private bool _backwards;
	
	// FixedUpdate is called once per physics update
	void FixedUpdate ()
    {
        float currLength = _timer.NormalizedTimeElapsed * _length;

        if (!_backwards)
        {
            for (int i = 0; i < _currentObjectNum; ++i)
            {
                currLength -= (_transform[i].position - _transform[i + 1].position).magnitude;
            }
        }
        else
        {
            for (int i = _transform.Count - 1; i > _currentObjectNum; --i)
            {
                currLength -= (_transform[i].position - _transform[i - 1].position).magnitude;
            }
        }

        _fracTime = currLength / (_transform[_currentObjectNum].position - _transform[_nextObjectNum].position).magnitude;

        if (!_backwards)
        {

            transform.position =
                                Vector3.Lerp(_transform[_currentObjectNum].position
                                            , _transform[_nextObjectNum].position, _fracTime);


        }
        else
        {

            transform.position =
                                Vector3.Lerp(_transform[_currentObjectNum].position
                                            , _transform[_nextObjectNum].position, _fracTime);


        }

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

        if (!_backwards)
        {
            if (_nextObjectNum < _transform.Count - 1)
            {
                _currentObjectNum = _nextObjectNum;
                _nextObjectNum += 1;
            }

        }
        else
        {
            if (_nextObjectNum > 0 )
            {
                _currentObjectNum = _nextObjectNum;
                _nextObjectNum -= 1;
            }
            
        }
    }

    public override void TimedAction()
    {
        _backwards = !_backwards;
    }
}
