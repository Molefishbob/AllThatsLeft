using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : CharControlBase, ITimedAction
{
    private float _time = 0;
    public float _circleRadius = 1;
    private bool _stopMoving = false;
    private OneShotTimer _timer;

    protected override void Awake()
    {
        base.Awake();
        _timer = GetComponent<OneShotTimer>();
        _timer.SetTimerTarget(this);
    }


    protected override Vector3 InternalMovement()
    {

        float x, y, z;

        if (!_stopMoving)
        {
            _time += Time.deltaTime / _circleRadius;

            x = Mathf.Sin(_time);
            y = 0;
            z = Mathf.Cos(_time);
        }
        else
        {
            x = 0;
            y = 0;
            z = 0;
        }

        if(x > 0.9999f || x < -0.9999f || z > 0.9999f || z < -0.9999f)
        {
            _stopMoving = true;
            _timer.StartTimer(2);
        }

        Vector3 move = new Vector3(x,y,z);
        Debug.Log(x);
        
        return move;
    }

    public void TimedAction()
    {
        _stopMoving = false;
    }
}
