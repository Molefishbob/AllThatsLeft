using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenBackForth : GenericMover {

	private bool _backwards;
    private float time;
	
	// FixedUpdate is called once per physics update
	void FixedUpdate ()
    {

        time += Time.deltaTime;


        _fracTime = (Time.time - _eventTime) / (_duration *
                (_transform[_currentObjectNum].position - _transform[_nextObjectNum].position).magnitude / _length);
        
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
                                            ,_transform[_nextObjectNum].position,_fracTime);
            

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

        if (_currentObjectNum >= _amountOfTransforms - 1 && !_backwards)
        {
            _backwards = true;
            _trackRecord++;
            _eventTime = _ogStartTime + _trackRecord * _duration;
        }
        else if (_currentObjectNum <= 1 && _backwards)
        {
            _backwards = false;
            _trackRecord++;
            _eventTime = _ogStartTime + _trackRecord * _duration;
        }
        else
        {
            _eventTime = Time.time;
        }

        if (!_backwards)
        {
            _currentObjectNum = _nextObjectNum;
            _nextObjectNum += 1;
            
        }
        else
        {
            _currentObjectNum = _nextObjectNum;
            _nextObjectNum -= 1;
        }
    }
}
