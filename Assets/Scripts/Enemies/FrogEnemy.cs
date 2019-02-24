using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : CharControlBase, ITimedAction
{
    private float _time = 0;
    public float _circleRadius = 1;
    private bool _stopMoving, _nextStopX, _nextStopZ, _followPlayer;
    private OneShotTimer _timer;
    private Vector3 _goBackPosition, _playerPosition;


    protected override void Awake()
    {
        base.Awake();
        _timer = GetComponent<OneShotTimer>();
        _timer.SetTimerTarget(this);
        _stopMoving = false;
        _nextStopX = true;
        _nextStopZ = false;
        _followPlayer = false;
    }


    protected override Vector3 InternalMovement()
    {
        float x, y, z;

        if (!_stopMoving && !_followPlayer)
        {
            _time += Time.deltaTime / _circleRadius;

            x = Mathf.Sin(_time);
            y = 0;
            z = Mathf.Cos(_time);
        }
        else if (!_stopMoving && _followPlayer)
        {
            Vector3 goToPlayer = _playerPosition - transform.position;
            x = _playerPosition.x;
            y = 0;
            z = _playerPosition.y;
        }
        else 
        {
            x = 0;
            y = 0;
            z = 0;
        }

        if(_nextStopX && (x > 0.999f || x < -0.999f))
        {
            _nextStopX = false;
            _nextStopZ = true;
            _stopMoving = true;
            _timer.StartTimer(2);
 
        }else if (_nextStopZ && (z > 0.999f || z < -0.999f))
        {
            _nextStopX = true;
            _nextStopZ = false;
            _stopMoving = true;
            _timer.StartTimer(2);   
        }
        Vector3 move = new Vector3(x,y,z);
        Debug.Log(_followPlayer);
        return move;
    }

    public void TimedAction()
    {
        _stopMoving = false;
    }

    //TODO: make it follow player correctly. Maybe use spherecast instead
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            _followPlayer = true;
            _goBackPosition = transform.position;
            _playerPosition = other.gameObject.transform.position;
        }
        else
        {
            _followPlayer = false;
        }
    }
}
