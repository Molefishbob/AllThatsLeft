using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharMovement : PlayerMovement, IDamageReceiver
{
    public event GenericEvent OnPlayerAlive;
    public event GenericEvent OnPlayerDeath;

    [SerializeField]
    protected string _animatorTriggerDeath = "Die";
    [SerializeField]
    protected string _animatorTriggerAlive = "Default";
    [SerializeField]
    protected float _deathTime = 5.0f;
    [SerializeField]
    protected RandomSFXSound _damageDeathSound = null;
    [SerializeField]
    protected SingleSFXSound _fallDeathSound = null;

    public ParticleSystem _teleportEffectSlow;
    public ParticleSystem _teleportEffectFast;

    protected ScaledOneShotTimer _deathTimer;

    public bool Dead { get; protected set; }

    public override bool ControlsDisabled
    {
        get
        {
            return _controlsDisabled || Dead;
        }
        set
        {
            if (!Dead)
            {
                _controlsDisabled = value;
                if (Jump != null)
                {
                    Jump.ControlsDisabled = value;
                }
                if (_controlsDisabled)
                {
                    ResetInternalMove();
                }
            }
        }
    }

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
        if (Dead) return;

        GameManager.Instance.Camera.MoveToTargetInstant(transform);
        Dead = true;
        _damageDeathSound.PlaySound();
        _animator.SetTrigger(_animatorTriggerDeath);
        SetControllerActive(false);
        _deathTimer.StartTimer(_deathTime);
        if (OnPlayerDeath != null) OnPlayerDeath();
    }

    private void Alive()
    {
        SetControllerActive(false);
        GameManager.Instance.LevelManager.ResetLevel();
        Dead = false;
        _animator.SetTrigger(_animatorTriggerAlive);
        SetControllerActive(true);
        if (OnPlayerAlive != null) OnPlayerAlive();
    }

    protected override void OutOfBounds()
    {
        if (Dead) return;

        Dead = true;
        _fallDeathSound.PlaySound();
        _deathTimer.StartTimer(_deathTime);
        if (OnPlayerDeath != null) OnPlayerDeath();
    }

}
