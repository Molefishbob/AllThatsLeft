using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharMovement : PlayerMovement, IDamageReceiver
{
    [SerializeField]
    protected string _animatorBoolDeath = "Dead";
    [SerializeField]
    protected float _deathTime = 5.0f;
    protected bool _dead = false;
    protected ScaledOneShotTimer _deathTimer;
    protected override void Awake()
    {
        base.Awake();
        _deathTimer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    protected override void Start()
    {
        base.Start();
        _deathTimer.OnTimerCompleted += Alive;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_deathTimer != null)
        {
            _deathTimer.OnTimerCompleted -= Alive;
        }
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

    private void Alive()
    {
        transform.position = GameManager.Instance.LevelManager.GetSpawnLocation();
        _dead = false;
        ControlsDisabled = false;
        _animator?.SetBool(_animatorBoolDeath, false);
        SetControllerActive(true);
    }
}