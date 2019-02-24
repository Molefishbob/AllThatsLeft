﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : GenericHackable, ITimedAction
{
    [SerializeField, Tooltip("The amount of time needed to hack")]
    protected float _duration = 0.5f;

    private OneShotTimer _timer;

    protected override void Awake()
    {
        base.Awake();
        _timer = GetComponent<OneShotTimer>();
    }
    private void Start()
    {
        _timer.SetTimerTarget(this);
    }
    /// <summary>
    /// Determines the actions when something starts to hack the object.
    /// By default this is called when something enters the trigger.
    /// </summary>
    protected override void StartHack()
    {
        if (_currentStatus == Status.NotHacked) 
        {
            StartTimer(_duration);
            _currentStatus = Status.BeingHacked;
        }
    }
    /// <summary>
    /// Determines the actions when something stops to hack the object.
    /// By default this is called when something leaves the trigger and there is nothing else hacking the console.
    /// </summary>
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
    /// <summary>
    /// Determines what the console does when it is being hacked or has been hacked.
    /// For example you can call the targets methods here.
    /// </summary>
    protected override void HackAction()
    {
        _hTarget.ButtonDown();
    }
    /// <summary>
    /// Starts the timer for the duration of the hack.
    /// If the timer is already running it will not restart the timer.
    /// </summary>
    /// <param name="duration">The duration of the timer</param>
    /// <returns>Was the timer successfully started</returns>
    protected bool StartTimer(float duration)
    {
        if (!_timer.IsRunning)
        {
            _timer.StartTimer(duration);
            return true;
        }
        else
        {
            return false;
        }

    }
    /// <summary>
    /// Called by the timer once it has completed.
    /// Determines what happens when the timer has been completed.
    /// </summary>
    public void TimedAction()
    {
        switch(_currentStatus)
        {
            case Status.BeingHacked:
                _currentStatus = Status.Hacked;
                for (int a = 0; a < _hackers.Count; a++) {
                    _hackers[a].ResetBot();
                }
                _hackers.Clear();
                HackAction();
                break;
            default:
                Debug.LogError("Current Status:" + _currentStatus + " Timer completed even though it shouldn't! ree");
                break;
        }
    }
}