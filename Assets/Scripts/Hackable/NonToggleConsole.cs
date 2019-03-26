﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonToggleConsole : GenericHackable
{

    /// <summary>
    /// Determines what the console does when it is being hacked or has been hacked.
    /// For example you can call the targets methods here.
    /// </summary>
    protected override void HackAction()
    {
        switch (_currentStatus)
        {
            case Status.BeingHacked:
                _hTarget.ButtonDown();
                break;
            case Status.NotHacked:
                _hTarget.ButtonUp();
                break;
        }
    }

    /// <summary>
    /// Determines the actions when something starts to hack the object.
    /// By default this is called when something enters the trigger.
    /// </summary>
    protected override void StartHack()
    {
        if (_currentStatus == Status.NotHacked)
        {
            _currentStatus = Status.BeingHacked;
            HackAction();
        }
    }

    /// <summary>
    /// Determines the actions when something stops to hack the object.
    /// By default this is called when something leaves the trigger and there is nothing else hacking the console.
    /// </summary>
    protected override void StopHack()
    {
        if (_currentStatus == Status.BeingHacked)
        {
            _currentStatus = Status.NotHacked;
            HackAction();
        }
    }

}