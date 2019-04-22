using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericHackable : MonoBehaviour
{
    public event GenericEvent OnHackSuccess;

    public enum Status
    {
        NotHacked,
        Hacked,
        BeingHacked
    }
    [SerializeField, Tooltip("The starting status of the hackable object")]
    protected Status _startingStatus;
    [SerializeField, Tooltip("The target that will be called once hacked.\nTarget HAS TO IMPLEMENT IButtonInteraction!")]
    protected MonoBehaviour _hackTarget = null;

    protected Status _currentStatus;
    public Status CurrentStatus
    {
        get
        {
            return _currentStatus;
        }
        protected set
        {
            _currentStatus = value;
            if(_currentStatus==Status.Hacked && OnHackSuccess!=null)OnHackSuccess();
        }
    }

    protected IButtonInteraction _hTarget;

    protected virtual void Awake()
    {
        CurrentStatus = _startingStatus;
        try
        {
            _hTarget = (IButtonInteraction)_hackTarget;
        }
        catch
        {
            Debug.LogError("The Target of " + gameObject.name + " " + transform.position + " HAS TO IMPLEMENT IButtonInteraction");
        }

    }

    /// <summary>
    /// Called when a minibot enters the hacking radius of the console
    /// </summary>
    /// <param name="bot">The minibot that entered the radius</param>
    public virtual void TimeToStart()
    {
        StartHack();
    }

    /// <summary>
    /// Called when the minibot leaves the hacking radius of the console
    /// </summary>
    /// <param name="bot">The minibot that entered the radius</param>
    public virtual void TimeToLeave()
    {
        StopHack();
    }
    /// <summary>
    /// Determines the actions when something starts to hack the object.
    /// By default this is called when something enters the trigger.
    /// </summary>
    protected abstract void StartHack();
    /// <summary>
    /// Determines the actions when something stops to hack the object.
    /// By default this is called when something leaves the trigger and there is nothing else hacking the console.
    /// </summary>
    protected abstract void StopHack();
    /// <summary>
    /// Determines what the console does when it is being hacked or has been hacked.
    /// For example you can call the targets methods here.
    /// </summary>
    protected abstract void HackAction();
}
