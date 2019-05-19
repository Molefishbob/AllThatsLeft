using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenBackForth : GenericMover
{
    [SerializeField, Tooltip("The amount of time the platform is still at the ends of the route")]
    protected float _stopTime;
    [SerializeField]
    protected GameObject _endPoint = null;
    private bool _backwards;
    private float _viggleOffsetMultiplier = 2f;
    private bool _stop;
    private float _stopCounter;
    private Vector3 _endPointOffset = new Vector3(0,-0.45f,0);

    protected override void Awake()
    {
        base.Awake();
        _duration += _stopTime;
    }

    protected override void Start()
    {
        base.Start();
        _animationLength = _anim.runtimeAnimatorController.animationClips[0].length;
        _viggleTimer.OnTimerCompleted += StartViggle;
        if (_endPoint != null)
        {
            Instantiate(_endPoint, _transform[_transform.Count - 1].position + _endPointOffset, _endPoint.transform.rotation, _transform[_transform.Count - 1]);
        } else
        {
            Debug.LogError("EndPoint should not be null");
        }
    }

    public override void Init()
    {
        base.Init();
        if (_stopTime - _animationLength * _viggleOffsetMultiplier <= 0)
        {
            StartViggle();
        } else
        {
            _viggleTimer.StartTimer(_stopTime - (_animationLength * _viggleOffsetMultiplier));
        }
    }

    protected override Vector3 InternalMove()
    {
        if (_activated)
        {
            if (_timer.TimeElapsed > _stopTime)
            {
                Vector3 pos;
                float currLength = (_timer.TimeElapsed - _stopTime) / (_timer.Duration - _stopTime) * _length;

                if (!_backwards)
                {
                    for (int i = 0; i < _currentObjectNum; ++i)
                    {
                        currLength -= (_transform[i].position - _transform[i + 1].position).magnitude;
                    }
                    _fracTime = currLength / (_transform[_currentObjectNum].position - _transform[_currentObjectNum + 1].position).magnitude;
                }
                else
                {
                    for (int i = _transform.Count - 1; i > _currentObjectNum; --i)
                    {
                        currLength -= (_transform[i].position - _transform[i - 1].position).magnitude;
                    }
                    _fracTime = currLength / (_transform[_currentObjectNum].position - _transform[_currentObjectNum - 1].position).magnitude;
                }

                if (!_backwards)
                {
                    pos = Vector3.Lerp(_transform[_currentObjectNum].position, _transform[_currentObjectNum + 1].position, _fracTime);
                }
                else
                {
                    pos = Vector3.Lerp(_transform[_currentObjectNum].position, _transform[_currentObjectNum - 1].position, _fracTime);
                }

                if (_fracTime >= 1)
                {
                    ChangeTarget();
                }

                return pos;
            } 
        }

        return transform.position;
    }


    /// <summary>
    /// Changes the direction the object is travelling
    /// </summary>
    private void ChangeTarget()
    {

        if (!_backwards)
        {
            if (_currentObjectNum < _transform.Count - 1)
            {
                _currentObjectNum++;
            }

        }
        else
        {
            if (_currentObjectNum > 0)
            {
                _currentObjectNum--;
            }

        }
    }

    /// <summary>
    /// Called when the timer is completed.
    /// 
    /// Defines what happens when the timer has completed.
    /// </summary>
    protected override void TimedAction()
    {
        if (_stopTime - _animationLength * _viggleOffsetMultiplier <= 0)
        {
            StartViggle();
        }
        else
        {
            _viggleTimer.StartTimer(_stopTime - (_animationLength * _viggleOffsetMultiplier));
        }
        if (!_backwards)
        {
            _backwards = true;
            _currentObjectNum = _transform.Count - 1;
        }
        else
        {
            _backwards = false;
            _currentObjectNum = 0;
        }
    }
}
