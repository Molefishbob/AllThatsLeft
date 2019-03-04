using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericHackable : MonoBehaviour
{
    public enum Status
    {
        NotHacked,
        Hacked,
        BeingHacked
    }
    [SerializeField,Tooltip("The starting status of the hackable object")]
    protected Status _startingStatus;
    [SerializeField, Tooltip("The target that will be called once hacked.\nTarget HAS TO IMPLEMENT IButtonInteraction!")]
    private MonoBehaviour _hackTarget = null;
    public Status _currentStatus
    {
        get;
        protected set;
    }
    
    protected IButtonInteraction _hTarget;
    protected List<HackerBot> _hackers;

    protected virtual void Awake()
    {
        _hackers = new List<HackerBot>();
        _currentStatus = _startingStatus;
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
    /// Called when an object enters the triggers collision area
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        _hackers.Add(other.GetComponent<HackerBot>());
        StartHack();
    }

    /// <summary>
    /// Called when an object leaves the triggers collision area
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerExit(Collider other)
    {
        _hackers.Remove(other.GetComponent<HackerBot>());
        if (_hackers.Count <= 0)
        {
            StopHack();
        }
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
