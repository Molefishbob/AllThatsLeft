using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotReleaser : BotActionBase
{
    [SerializeField]
    private float _fReleaseDelay = 2.0f;
    [SerializeField]
    private float _transitionTime = 1.0f;
    [SerializeField]
    private float _transitionTimeOnPlayerDeath = 0.5f;
    [SerializeField]
    private float _fLifeTime = 30;

    private LayerMask _lHackableLayer = 1 << 18;

    private ScaledOneShotTimer _ostRelease;
    private ScaledOneShotTimer _ostDisable;
    private PhysicsOneShotTimer _ostLife;

    private BombAction _selfBomb;
    private HackAction _selfHack;
    private TrampolineAction _selfTrampoline;

    protected override void Awake()
    {
        base.Awake();

        _ostRelease = gameObject.AddComponent<ScaledOneShotTimer>();
        _ostDisable = gameObject.AddComponent<ScaledOneShotTimer>();
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
        _ostLife.OnTimerCompleted += _selfMover.Die;

        if (!_selfMover.ControlsDisabled)
            Activate();
    }

    void OnDisable()
    {
        _ostDisable.StopTimer();
        _ostRelease.StopTimer();
        _ostLife.StopTimer();
        _selfMover._animator.SetBool("Explode", false);
        if (GameManager.Instance != null && GameManager.Instance.Player != null) GameManager.Instance.Player.OnPlayerDeath -= ReleaseInstant;
    }

    public void Activate()
    {
        _selfMover.SetControllerActive(true);
        _selfBomb._bCanAct = true;
        _selfHack._bCanAct = true;
        _selfTrampoline._bCanAct = true;
    }

    public void ReleaseControls(bool withDelay)
    {
        _selfMover.ControlsDisabled = true;
        DisableActing();
        GameObject[] hacks = CheckSurroundings(_lHackableLayer, _selfMover._controller.radius + _selfMover._controller.skinWidth);
        if (hacks != null && hacks.Length > 0)
        {
            foreach (GameObject item in hacks)
            {
                GenericHackable hack = item.GetComponent<GenericHackable>();
                hack?.ShowPrompt(false);
            }
        }
        if (withDelay)
        {
            _ostRelease.StartTimer(_fReleaseDelay);
        }
        else
        {
            ActualRelease();
        }
    }

    public void ReleaseInstant()
    {
        _selfMover.ControlsDisabled = true;
        DisableActing();
        GameObject[] hacks = CheckSurroundings(_lHackableLayer, _selfMover._controller.radius + _selfMover._controller.skinWidth);
        if (hacks != null && hacks.Length > 0)
        {
            foreach (GameObject item in hacks)
            {
                GenericHackable hack = item.GetComponent<GenericHackable>();
                hack?.ShowPrompt(false);
            }
        }
        GameManager.Instance.Camera.GetNewTarget(GameManager.Instance.Player.transform, _transitionTimeOnPlayerDeath, true);
        _ostDisable.StartTimer(_transitionTimeOnPlayerDeath);
    }

    private void ActualRelease()
    {
        GameManager.Instance.Camera.GetNewTarget(GameManager.Instance.Player.transform, _transitionTime, true);
        if (!_selfTrampoline._bActing)
            _ostDisable.StartTimer(_transitionTime);
        else
            _ostDisable.StartTimer(_fLifeTime);
    }

    public override void DisableAction()
    {
        _selfHack.DisableAction();
        _selfBomb.DisableAction();
        _selfTrampoline.DisableAction();

        if (!_selfMover.ControlsDisabled)
        {
            GameManager.Instance.Player.ControlsDisabled = false;
            _selfMover.ControlsDisabled = true;
        }
        if (!_ostLife.IsRunning)
            gameObject.SetActive(false);
    }

    public void DisableActing()
    {
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
        if (_ostLife != null && _selfMover != null)
        {
            _ostLife.OnTimerCompleted -= _selfMover.Die;
        }
    }
}
