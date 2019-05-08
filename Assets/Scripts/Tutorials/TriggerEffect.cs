using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEffect : MonoBehaviour
{
    [SerializeField]
    private GenericHackable _hidePermanentlyWhenHacked = null;

    private Collider _collider = null;
    private GameObject _effect = null;

    private BotMovement _botMover = null;
    private BotReleaser _botRelease = null;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _effect = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        _effect.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_botRelease != null)
        {
            _botRelease.OnBotReleased -= HideEffect;
        }

        if (_hidePermanentlyWhenHacked != null)
        {
            _hidePermanentlyWhenHacked.OnHackSuccess -= DisableCompletely;
        }
    }

    private void FixedUpdate()
    {
        if (_botMover != null && (!_botMover.gameObject.activeInHierarchy || _botMover.ControlsDisabled))
        {
            HideEffect();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_botMover != null)
        {
            return;
        }

        BotMovement foundMover = other.GetComponent<BotMovement>();

        if (foundMover == null || foundMover.ControlsDisabled)
        {
            return;
        }

        _botMover = foundMover;
        _botRelease = other.GetComponent<BotReleaser>();

        _effect.gameObject.SetActive(true);

        _botRelease.OnBotReleased += HideEffect;

        if (_hidePermanentlyWhenHacked != null)
        {
            _hidePermanentlyWhenHacked.OnHackSuccess += DisableCompletely;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_botMover == null)
        {
            return;
        }

        BotMovement foundBot = other.GetComponent<BotMovement>();

        if (foundBot == null || foundBot != _botMover)
        {
            return;
        }

        HideEffect();
    }

    private void HideEffect()
    {
        if (_botRelease != null)
        {
            _botRelease.OnBotReleased -= HideEffect;
        }

        _botMover = null;
        _botRelease = null;
        _effect.gameObject.SetActive(false);
    }

    private void DisableCompletely()
    {
        HideEffect();
        _hidePermanentlyWhenHacked.OnHackSuccess -= DisableCompletely;
        gameObject.SetActive(false);
    }
}
