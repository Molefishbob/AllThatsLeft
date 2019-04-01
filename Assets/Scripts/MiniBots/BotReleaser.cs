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
    }

    private void Start()
    {
        _ostRelease.OnTimerCompleted += ActualRelease;
        _ostDisable.OnTimerCompleted += DisableAction;
        _ostLife.OnTimerCompleted += _selfMover.Die;
    }

    public void ReleaseControls(bool withDelay)
    {
        _selfMover.ControlsDisabled = true;
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
        _ostDisable.StartTimer(_transitionTime);
    }
/* 
    private void DisableSelf()
    {
        GameManager.Instance.Player.ControlsDisabled = false;
        if (!_bHacking && !_ostLife.IsRunning)
        {
            if (_psExplosion != null)
            {
                _goParticleHolder.transform.parent = transform;
                _goParticleHolder.transform.localPosition = Vector3.zero;
            }
            _goTrampoline.SetActive(false);
            gameObject.SetActive(false);
        }
    }
 */
    public override void DisableAction()
    {
        _selfHack.DisableAction();
        _selfBomb.DisableAction();
        _selfTrampoline.DisableAction();
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
