using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericHackable : MonoBehaviour, ITimedAction
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
    [SerializeField, Tooltip("The amount of time needed to hack")]
    protected float _duration = 0.5f;
    [SerializeField, Tooltip("DO NOT TOUCH IF YOU DO NOT KNOW WHAT YOU ARE DOING")]
    protected int _hackerBotLayer = 14;
    public Status _currentStatus
    {
        get;
        protected set;
    }

    protected int _botsHacking = 0;
    protected IButtonInteraction _hTarget;
    protected OneShotTimer _timer;
    protected List<HackerBot> _hackers;

    protected void Awake()
    {

        _currentStatus = _startingStatus;
        _timer = GetComponent<OneShotTimer>();
        try
        {
            _hTarget = (IButtonInteraction)_hackTarget;
        }
        catch
        {
            Debug.LogError("The Target of " + gameObject.name + " " + transform.position + " HAS TO IMPLEMENT IButtonInteraction");
        }
        
    }
    protected void Start() {
        _timer.SetTimerTarget(this);
    }
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


    protected void OnTriggerEnter(Collider other)
    {
        _hackers.Add(other.GetComponent<HackerBot>());
        _botsHacking++;
        StartHack();
    }

    protected void OnTriggerExit(Collider other)
    {
        _hackers.Remove(other.GetComponent<HackerBot>());
        _botsHacking--;
        if (_botsHacking <= 0)
        {
            _botsHacking = 0;
            StopHack();
        }
    }

    protected abstract void StartHack();
    protected abstract void StopHack();
    protected abstract void HackAction();
    public abstract void TimedAction();
}
