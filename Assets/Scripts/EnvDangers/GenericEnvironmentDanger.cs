using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericEnvironmentDanger : MonoBehaviour
{
    protected PhysicsOneShotTimer _timer;

    protected virtual void Awake()
    {
        _timer = UnityEngineExtensions.GetOrAddComponent<PhysicsOneShotTimer>(gameObject);
    }

    protected virtual void Start()
    {
        _timer.OnTimerCompleted += TimedAction;
    }

    protected virtual void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= TimedAction;
        }
    }

    protected void DoDamage(Collider other)
    {
        other.GetComponent<IDamageReceiver>().TakeDamage(100);
    }
    protected abstract void StopDamage();

    protected virtual void OnTriggerExit(Collider other)
    {
        StopDamage();
    }

    /// <summary>
    /// What happens after the timer has run out
    /// </summary>
    public abstract void TimedAction();
}
