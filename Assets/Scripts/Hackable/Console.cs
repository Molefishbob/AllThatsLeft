using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : GenericHackable
{
    protected override void StartHack()
    {
        if (_currentStatus == Status.NotHacked) 
        {
            StartTimer(_duration);
            _currentStatus = Status.BeingHacked;
        }
    }

    protected override void StopHack()
    {
        _timer.StopTimer();
    }

    public override void TimedAction()
    {
        switch(_currentStatus)
        {
            case Status.BeingHacked:
                _currentStatus = Status.Hacked;
                break;
            default:
                Debug.LogError("Current Status:" + _currentStatus + " Timer completed even though it shouldn't!");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _botsHacking++;
        StartHack();
    }

    private void OnTriggerExit(Collider other)
    {
        _botsHacking--;
        if (_botsHacking <= 0)
        {
            _botsHacking = 0;
            StopHack();
        }
    }
}
