using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotReleaser : BotActionBase, IDamageReceiver
{
    public event GenericEvent OnBotReleased;

    [SerializeField]
    private float _fReleaseDelay = 2.0f;
    [SerializeField]
    private float _transitionTime = 1.0f;
    [SerializeField]
    private float _transitionTimeOnPlayerDeath = 0.5f;
    [SerializeField]
    private float _fLifeTime = 30;

    [HideInInspector]
    public bool Dead { get { return _dead; } set { _dead = value; } }
    private bool _dead = false;

    private LayerMask _lHackableLayer = 1 << 18;

    private ScaledOneShotTimer _ostRelease;
    private ScaledOneShotTimer _ostDisable;
    private ScaledOneShotTimer _ostControlRelease;
    private PhysicsOneShotTimer _ostLife;

    // Could use a BotActionBase list/array for scaleability
    private BombAction _selfBomb;
    private HackAction _selfHack;
    private TrampolineAction _selfTrampoline;

    protected override void Awake()
    {
        base.Awake();

        _ostRelease = gameObject.AddComponent<ScaledOneShotTimer>();
        _ostDisable = gameObject.AddComponent<ScaledOneShotTimer>();
        _ostControlRelease = gameObject.AddComponent<ScaledOneShotTimer>();
        _ostLife = gameObject.AddComponent<PhysicsOneShotTimer>();

        _selfBomb = GetComponent<BombAction>();
        _selfHack = GetComponent<HackAction>();
        _selfTrampoline = GetComponent<TrampolineAction>();
    }

    void OnEnable()
    {
        if (GameManager.Instance.Player != null) GameManager.Instance.Player.OnPlayerDeath += ReleaseInstant;
    }

    private void Start()
    {
        _ostRelease.OnTimerCompleted += ActualRelease;
        _ostDisable.OnTimerCompleted += DisableAction;
        _ostControlRelease.OnTimerCompleted += EnablePlayerControls;
        _ostLife.OnTimerCompleted += Die;
        _lHackableLayer = _selfHack.HackLayer;

        if (!_selfMover.ControlsDisabled)
            Activate();
    }

    void OnDisable()
    {
        _ostDisable.StopTimer();
        _ostRelease.StopTimer();
        _ostLife.StopTimer();
        _selfMover._animator.SetBool("Explode", false);
        Dead = false;
        _selfMover.Dead = false;
        // TODO Add player jump reset to BotMovement
        // Right now happens in movements OnDisable not a fan of that
        //_selfMover._playerJump.ResetJump();
        _selfMover.SetControllerActive(false);
        _selfMover.ControlsDisabled = true;

        if (GameManager.Instance != null && GameManager.Instance.Player != null) GameManager.Instance.Player.OnPlayerDeath -= ReleaseInstant;
    }

    public void Activate()
    {
        _selfBomb._bCanAct = true;
        _selfHack._bCanAct = true;
        _selfTrampoline._bCanAct = true;
    }

    public void ReleaseControls(bool withDelay)
    {
        _selfMover.ControlsDisabled = true;
        //_ostControlRelease.StartTimer(_transitionTime * 1.1f);
        DisableActing();

        if (withDelay)
        {
            _ostRelease.StartTimer(_fReleaseDelay);
            _ostControlRelease.StartTimer(_fReleaseDelay + _transitionTime);
        }
        else
        {
            _ostControlRelease.StartTimer(_transitionTime * 0.9f);
            ActualRelease();
        }
        if (OnBotReleased != null) OnBotReleased();
    }

    public void EnablePlayerControls()
    {
        GameManager.Instance.Player.ControlsDisabled = false;
    }

    public void ReleaseInstant()
    {
        _selfMover.ControlsDisabled = true;
        DisableActing();

        GameManager.Instance.Camera.GetNewTarget(GameManager.Instance.Player.transform, _transitionTimeOnPlayerDeath, true);
        _ostDisable.StartTimer(_transitionTimeOnPlayerDeath);
    }

    private void ActualRelease()
    {
        GameManager.Instance.Camera.GetNewTarget(GameManager.Instance.Player.transform, _transitionTime, true);
        if (_selfTrampoline._bActing || _selfHack.Hacking)
        {
            _ostDisable.StartTimer(_fLifeTime);
        } 
        else
            _ostDisable.StartTimer(_transitionTime);
    }

    public override void DisableAction()
    {
        // Right now lets just do this the stupid way
        // Need to do some stuff BEFORE the object is disabled
        _selfHack.DisableAction();
        _selfBomb.DisableAction();
        _selfTrampoline.DisableAction();

        _selfMover.ControlsDisabled = true;
        if (!_ostLife.IsRunning)
            gameObject.SetActive(false);
    }

    public void DisableActing()
    {
        // More stupid stuff here
        // Is called from actions
        _bCanAct = false;
        _selfBomb._bCanAct = false;
        _selfHack._bCanAct = false;
        _selfTrampoline._bCanAct = false;
    }

    private void OnDestroy()
    {
        if (_ostRelease != null)
        {
            _ostRelease.OnTimerCompleted -= ActualRelease;
        }
        if (_ostDisable != null)
        {
            _ostDisable.OnTimerCompleted -= DisableAction;
        }
        if (_ostControlRelease != null)
        {
            _ostControlRelease.OnTimerCompleted -= EnablePlayerControls;
        }
        if (_ostLife != null && _selfMover != null)
        {
            _ostLife.OnTimerCompleted -= Die;
        }
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        if (Dead) return;
        Dead = true;
        _selfMover.Dead = true;
        if (!_selfMover.ControlsDisabled)
            ReleaseControls(false);
    }
}
