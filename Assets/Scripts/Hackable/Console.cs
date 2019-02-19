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
        switch (_currentStatus)
        {
            case Status.BeingHacked:
                _currentStatus = Status.NotHacked;
                _timer.StopTimer();
                break;
        }
    }

    protected override void HackAction()
    {
        _hTarget.ButtonDown();
    }

    public override void TimedAction()
    {
        switch(_currentStatus)
        {
            case Status.BeingHacked:
                _currentStatus = Status.Hacked;
                HackAction();
                break;
            default:
                Debug.LogError("Current Status:" + _currentStatus + " Timer completed even though it shouldn't! ree");
                break;
        }
    }
}
