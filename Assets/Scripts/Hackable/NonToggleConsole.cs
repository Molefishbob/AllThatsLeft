using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonToggleConsole : GenericHackable
{

    /// <summary>
    /// Determines what the console does when it is being hacked or has been hacked.
    /// For example you can call the targets methods here.
    /// </summary>
    protected override bool HackAction()
    {
        switch (CurrentStatus)
        {
            case Status.BeingHacked:
                _hTarget.ButtonDown(0);
                return true;
            case Status.NotHacked:
                _hTarget.ButtonUp();
                return false;
        }
        return false;
    }

    /// <summary>
    /// Determines the actions when something starts to hack the object.
    /// By default this is called when something enters the trigger.
    /// </summary>
    protected override void StartHack()
    {
        if (CurrentStatus == Status.NotHacked)
        {
            CurrentStatus = Status.BeingHacked;
            HackAction();
        }
    }

    /// <summary>
    /// Determines the actions when something stops to hack the object.
    /// By default this is called when something leaves the trigger and there is nothing else hacking the console.
    /// </summary>
    protected override void StopHack()
    {
        if (CurrentStatus == Status.BeingHacked)
        {
            CurrentStatus = Status.NotHacked;
            HackAction();
        }
    }

}
