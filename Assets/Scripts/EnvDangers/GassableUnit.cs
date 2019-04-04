using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GassableUnit : MonoBehaviour
{
    private PhysicsOneShotTimer _timer;
    private IDamageReceiver _dmg;

    private void Awake()
    {
        _timer = gameObject.AddComponent<PhysicsOneShotTimer>();
        _dmg = GetComponent<IDamageReceiver>();
    }

    private void Start()
    {
        _timer.OnTimerCompleted += _dmg.Die;
    }

    private void OnDestroy()
    {
        if (_timer != null && _dmg != null)
        {
            _timer.OnTimerCompleted -= _dmg.Die;
        }
    }

    /// <summary>
    /// Start timer when entering gas.
    /// </summary>
    /// <param name="oofTime">Time until oof</param>
    public void EnterGas(float oofTime)
    {
        _timer.StartTimer(oofTime);
    }

    /// <summary>
    /// Stop timer when leaving gas.
    /// </summary>
    public void ExitGas()
    {
        _timer.StopTimer();
    }
}
