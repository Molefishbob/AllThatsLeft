﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : GenericHackable
{
    [SerializeField, Tooltip("The amount of time needed to hack")]
    protected float _duration = 0.5f;
    [SerializeField]
    protected float _lookAtHackedTime = 1.0f;
    [SerializeField]
    protected float _transitionTime = 0.5f;

    private PhysicsOneShotTimer _timer;

    protected override void Awake()
    {
        base.Awake();
        _timer = UnityEngineExtensions.GetOrAddComponent<PhysicsOneShotTimer>(gameObject);
    }
    protected virtual void Start()
    {
        _timer.OnTimerCompleted += CompleteHack;
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= CompleteHack;
        }
    }
    
    /// <summary>
    /// Determines the actions when something starts to hack the object.
    /// By default this is called when something enters the trigger.
    /// </summary>
    protected override void StartHack()
    {
        if (CurrentStatus == Status.NotHacked) 
        {
            StartTimer(_duration);
            CurrentStatus = Status.BeingHacked;
        }
    }
    
    /// <summary>
    /// Determines the actions when something stops to hack the object.
    /// By default this is called when something leaves the trigger and there is nothing else hacking the console.
    /// </summary>
    protected override void StopHack()
    {
        _timer.StopTimer();
        switch (CurrentStatus)
        {
            case Status.BeingHacked:
                CurrentStatus = Status.NotHacked;
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
    protected void CompleteHack()
    {
        switch(CurrentStatus)
        {
            case Status.BeingHacked:
                // TODO: ANIMATION HERE
                CurrentStatus = Status.Hacked;
                HackAction();
                GameManager.Instance.Camera.MoveToHackTargetInstant(_hackTarget.transform, _lookAtHackedTime, _transitionTime);
                break;
            default:
                Debug.LogError("Current Status:" + CurrentStatus + " Timer completed even though it shouldn't! ree");
                break;
        }
    }

}
