using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharControlBase, IDamageReceiver, ITimedAction
{
    [SerializeField]
    protected string _horizontalAxis = "Horizontal";
    [SerializeField]
    protected string _verticalAxis = "Vertical";
    [SerializeField]
    protected string _animatorBoolDeath = "Dead";
    [SerializeField]
    protected float _deathTime = 5.0f;

    [HideInInspector]
    public bool ControlsDisabled
    {
        get
        {
            return _controlsDisabled;
        }
        set
        {
            _controlsDisabled = value;
            if (_playerJump != null)
            {
                _playerJump.ControlsDisabled = value;
            }
        }
    }

    protected bool _controlsDisabled;
    protected bool _dead = false;
    protected OneShotTimer _deathTimer;
    protected PlayerJump _playerJump;

    protected override void Awake()
    {
        base.Awake();
        _playerJump = GetComponent<PlayerJump>();
        _deathTimer = gameObject.AddComponent<OneShotTimer>();
    }

    protected override void Start()
    {
        base.Start();
        _deathTimer.SetTimerTarget(this);
    }

    protected override Vector3 InternalMovement()
    {
        if (ControlsDisabled)
        {
            return Vector3.zero;
        }

        // read input
        float horizontal = Input.GetAxis(_horizontalAxis);
        float vertical = Input.GetAxis(_verticalAxis);

        // create combined vector of input
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        // clamp magnitude
        float desiredSpeed = Mathf.Clamp(inputDirection.magnitude, 0.0f, 1.0f);

        // convert to world space relative to camera
        inputDirection = GameManager.Instance.Camera.transform.TransformDirection(inputDirection);

        // remove pitch
        inputDirection.y = 0;

        // apply magnitude
        inputDirection = inputDirection.normalized * desiredSpeed;

        return inputDirection;
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
        transform.position = GameManager.Instance.LevelManager.GetSpawnLocation();
        _dead = false;
        ControlsDisabled = false;
        _animator?.SetBool(_animatorBoolDeath, false);
        SetControllerActive(true);
    }
}
