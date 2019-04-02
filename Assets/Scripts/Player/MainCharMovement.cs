using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharMovement : PlayerMovement, IDamageReceiver
{
    public event GenericEvent OnPlayerAlive;
    public event GenericEvent OnPlayerDeath;

    [SerializeField]
    protected string _animatorBoolDeath = "Dead";
    [SerializeField]
    protected float _deathTime = 5.0f;
    protected ScaledOneShotTimer _deathTimer;

    [HideInInspector]
    public bool Dead { get; protected set; }

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

    private void OnDestroy()
    {
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
        if (Dead) return;

        Dead = true;
        ControlsDisabled = true;
        _animator?.SetBool(_animatorBoolDeath, true);
        SetControllerActive(false);
        _playerJump.ResetJump();
        _deathTimer.StartTimer(_deathTime);
        if (OnPlayerDeath != null) OnPlayerDeath();
    }

    private void Alive()
    {
        SetControllerActive(false);
        GameManager.Instance.LevelManager.ResetLevel();
        //transform.position = GameManager.Instance.LevelManager.GetSpawnLocation();
        Dead = false;
        ControlsDisabled = false;
        _animator?.SetBool(_animatorBoolDeath, false);
        SetControllerActive(true);
        if (OnPlayerAlive != null) OnPlayerAlive();
    }

    protected override void OutOfBounds()
    {
        if (Dead) return;

        Dead = true;
        ControlsDisabled = true;
        _deathTimer.StartTimer(_deathTime);
        if (OnPlayerDeath != null) OnPlayerDeath();
    }
}