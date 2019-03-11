using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : GenericEnvironmentDanger
{
    [SerializeField, Tooltip("Time until the unit dies inside the area")]
    private float _timeUntilOof = 2;
    private Collider _other;
    public override void TimedAction()
    {
        if (_other != null)
            DoDamage(_other);
        _other = null;
    }
    
    protected override void StopDamage()
    {
        _timer.StopTimer();
    }

    private void OnTriggerEnter(Collider other)
    {
        _timer.StartTimer(_timeUntilOof);
        _other = other;
    }

    protected override void OnTriggerExit(Collider other)
    {
        StopDamage();
        _other = null;
    }
}
