using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharMovement : PlayerMovement, IDamageReceiver, ITimedAction
{
    [SerializeField]
    protected string _animatorBoolDeath = "Dead";
    [SerializeField]
    protected float _deathTime = 5.0f;
    protected bool _dead = false;
    protected OneShotTimer _deathTimer;
    protected override void Awake()
    {
        base.Awake();
        _deathTimer = gameObject.AddComponent<OneShotTimer>();
    }

    protected override void Start()
    {
        base.Start();
        _deathTimer.SetTimerTarget(this);
    }

    public virtual void TakeDamage(int damage)
    {
        Die();
    }

    public virtual void Die()
    {
        if (!_dead)
        {
            _dead = true;
            ControlsDisabled = true;
            _animator?.SetBool(_animatorBoolDeath, true);
            SetControllerActive(false);
            _deathTimer.StartTimer(_deathTime);
        }
    }

    public void TimedAction()
    {
        GameManager.Instance.LevelManager.ResetLevel();
        //transform.position = GameManager.Instance.LevelManager.GetSpawnLocation();
        _dead = false;
        ControlsDisabled = false;
        _animator?.SetBool(_animatorBoolDeath, false);
        SetControllerActive(true);
    }
}