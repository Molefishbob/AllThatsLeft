using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : PlayerMovement
{
    private PlayerBotInteractions _pbi;
    protected override void Awake()
    {
        base.Awake();
        _pbi = GetComponent<PlayerBotInteractions>();
    }

    public override void Die()
    {
        // TODO animations
        _pbi.StopActing();
        if (_pbi._bActive) _pbi.ReleaseControls();
    }
}
