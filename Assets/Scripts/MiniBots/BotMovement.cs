using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : PlayerMovement, IDamageReceiver
{
    [HideInInspector]
    public bool Dead { get; private set; }
    private PlayerBotInteractions _pbi;
    protected override void Awake()
    {
        base.Awake();
        _pbi = GetComponent<PlayerBotInteractions>();
    }

    private void OnDisable()
    {
        Dead = false;
        _playerJump.ResetJump();
        SetControllerActive(false);
        ControlsDisabled = true;
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        if (Dead) return;

        Dead = true;

        // TODO animations
        _pbi.StopActing();
    }

    protected override void OutOfBounds()
    {
        if (Dead) return;

        Dead = true;
        _pbi.ReleaseControls(false);
    }
}
