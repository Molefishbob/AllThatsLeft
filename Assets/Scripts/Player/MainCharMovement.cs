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
    private ThirdPersonCamera _cam;

    protected override void Awake()
    {
        base.Awake();
        _deathTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _cam = GameObject.Find("Camera(Clone)").GetComponent<ThirdPersonCamera>();
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
        SetControllerActive(false);
        GameManager.Instance.LevelManager.ResetLevel();
        //transform.position = GameManager.Instance.LevelManager.GetSpawnLocation();
        _dead = false;
        ControlsDisabled = false;
        _animator?.SetBool(_animatorBoolDeath, false);
        SetControllerActive(true);
        _cam.Following(true);
    }

    protected override void OutOfBounds()
    {
        if (!_dead)
        {
            _dead = true;
            ControlsDisabled = true;
            _deathTimer.StartTimer(_deathTime);
            _cam.Following(false);
        }
    }
}