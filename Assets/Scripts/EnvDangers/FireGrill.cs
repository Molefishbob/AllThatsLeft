using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGrill : GenericEnvironmentDanger
{
    public enum Status
    {
        Fire,
        NoFire
    }
    [SerializeField,Tooltip("The current status of the fire trap")]
    protected Status _currentStatus;
    [SerializeField, Tooltip("The time between fires")]
    protected float _fireInterval = 2;
    [SerializeField, Tooltip("The length of time that the fire burns for")]
    protected float _fireDuration = 1;
    [SerializeField, Tooltip("The delay after activated until the fire cycle starts")]
    protected float _startDelay = 0;
    [SerializeField, Tooltip("The gameobject under which are all the flames")]
    protected GameObject _flames = null;
    private bool _activated = false;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        if (_startDelay > 0)
        {
            _timer.StartTimer(_startDelay);
            _activated = false;
            _flames.SetActive(false);
        }
        else
        {
            _activated = true;
            _timer.StartTimer(_fireDuration);
            _flames.SetActive(true);
        }
    }
    

    public override void TimedAction()
    {
        if (!_activated)
        {
            _activated = true;
            _flames.SetActive(true);
            _timer.StartTimer(_fireDuration);
            _currentStatus = Status.Fire;

            return;
        }

        if (_currentStatus == Status.NoFire)
        {
            if (_startDelay > 0)
            {
                _activated = false;
                _timer.StartTimer(_startDelay);
            }
            else
            {
                _timer.StartTimer(_fireDuration);
                _flames.SetActive(true);
                _currentStatus = Status.Fire;
            }
        } else
        {
            if (_fireInterval > 0)
                _timer.StartTimer(_fireInterval);
            
            _flames.SetActive(false);
            _currentStatus = Status.NoFire;
        }
    }
    
    protected override void StopDamage()
    {
        throw new System.NotImplementedException();
    }

    protected void OnTriggerStay(Collider other)
    {
        if (_currentStatus == Status.Fire)
        {
            DoDamage(other);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (_currentStatus == Status.Fire)
        {
            StopDamage();
        }
    }
}
