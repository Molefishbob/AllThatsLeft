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

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        if (_fireInterval > 0)
            _timer.StartTimer(_fireInterval);
    }

    public override void TimedAction()
    {
        if (_currentStatus == Status.NoFire)
        {
            _timer.StartTimer(_fireDuration);
            _currentStatus = Status.Fire;
        } else
        {
            if (_fireInterval > 0)
                _timer.StartTimer(_fireInterval);

            _currentStatus = Status.NoFire;
        }
    }

    protected override void DoDamage(Collider other)
    {
        other.GetComponent<IDamageReceiver>().TakeDamage(100);
    }

    protected override void StopDamage()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnTriggerStay(Collider other)
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
