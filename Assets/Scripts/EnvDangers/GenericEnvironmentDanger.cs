using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnvironmentDanger : MonoBehaviour, ITimedAction
{
    protected OneShotTimer _timer;

    protected virtual void Awake()
    {
        _timer = UnityEngineExtensions.GetOrAddComponent<OneShotTimer>(gameObject);
    }

    protected virtual void Start()
    {
        _timer.SetTimerTarget(this);
    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected abstract void DoDamage(Collider other);
    protected abstract void StopDamage();

    protected virtual void OnTriggerStay(Collider other)
    {
        DoDamage(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        StopDamage();
    }

    /// <summary>
    /// What happens after the timer has run out
    /// </summary>
    public abstract void TimedAction();
}
