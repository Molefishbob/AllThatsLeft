using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotReleaser : BotActionBase, IDamageReceiver
{
    public event GenericEvent OnBotReleased;

    public float fReleaseDelay = 2.0f;
    [SerializeField]
    private float _transitionTime = 1.0f;
    [SerializeField]
    private float _transitionTimeOnPlayerDeath = 0.5f;
    [SerializeField]
    private float _fLifeTime = 30;

    [HideInInspector]
    public bool Dead { get; protected set; }

    private LayerMask _lHackableLayer = 1 << 18;

    private ScaledOneShotTimer _ostRelease;
    private ScaledOneShotTimer _ostDisable;
    private ScaledOneShotTimer _ostControlRelease;
    private PhysicsOneShotTimer _ostLife;

    // Could use a BotActionBase list/array for scaleability
    private BombAction _selfBomb;
    private HackAction _selfHack;
    [HideInInspector]
    public TrampolineAction _selfTrampoline;
    private Transform _thisCamTarget;
    public Transform ThisCameraTarget { get { return _thisCamTarget; } }

    [SerializeField]
    private GameObject teleportAwayParticle = null;

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

        _thisCamTarget = transform.Find("CameraTarget");
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
        _ostControlRelease.StopTimer();
        _ostLife.StopTimer();
        _selfMover._animator.SetBool("Explode", false);
        Dead = false;
        // TODO Add player jump reset to BotMovement
        // Right now happens in movements OnDisable not a fan of that
        //_selfMover._playerJump.ResetJump();
        _selfMover.SetControllerActive(false);
        _selfMover.ControlsDisabled = true;

        if (GameManager.Instance != null && GameManager.Instance.Player != null) GameManager.Instance.Player.OnPlayerDeath -= ReleaseInstant;
    }

    public void Activate()
    {
        _bCanAct = true;
        _selfBomb._bCanAct = true;
        _selfHack._bCanAct = true;
        _selfTrampoline._bCanAct = true;
    }

    public void ReleaseControls(bool withDelay)
    {
        GameManager.Instance.Player.OnPlayerDeath -= ReleaseInstant;

        DisableActing();

        if (_selfMover.ControlsDisabled)
        {
            _ostDisable.StartTimer(_transitionTime);
        }
        else if (withDelay)
        {
            _ostRelease.StartTimer(fReleaseDelay);
            _ostControlRelease.StartTimer(fReleaseDelay + _transitionTime * 0.9f);
        }
        else
        {
            _ostControlRelease.StartTimer(_transitionTime * 0.9f);
            ActualRelease();
        }

        _selfMover.ControlsDisabled = true;

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

        GameManager.Instance.Camera.MoveToTarget(GameManager.Instance.Player.transform, _transitionTimeOnPlayerDeath);
        _ostDisable.StartTimer(_transitionTimeOnPlayerDeath);
        if (!GameManager.Instance.Player.Dead)
            EnablePlayerControls();
    }

    private void ActualRelease()
    {
        if (GameManager.Instance.Camera.LookingAt == _thisCamTarget)
        {
            GameManager.Instance.Camera.MoveToTarget(GameManager.Instance.Player.transform, _transitionTime);
        }
        if (_selfTrampoline._bActing)
        {
            _ostLife.StartTimer(_fLifeTime);
        }
        else
            _ostDisable.StartTimer(_transitionTime);
    }

    public override void DisableAction()
    {
        _selfHack.DisableAction();
        _selfBomb.DisableAction();
        _selfTrampoline.DisableAction();
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
        if (Dead || _selfBomb._bExploding) return;

        Dead = true;

        // Why the fuck I didn't do this before?
        _ostLife.StopTimer();

        if (!_selfBomb._bExploding)
        {
            Instantiate(teleportAwayParticle, transform.position, Quaternion.identity);
        }

        ReleaseControls(true);
    }

    public void DeadButNotDead()
    {
        Dead = true;
    }

    public float GetRemainingLifeTime()
    {
        return _ostLife.TimeLeft;
    }
}
