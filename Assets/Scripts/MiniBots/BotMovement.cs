using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : PlayerMovement
{
    [HideInInspector]
    public bool Dead { get { return _dead; } set { _dead = value; } }
    private bool _dead = false;
    private BotReleaser _selfReleaser;
    protected override void Awake()
    {
        base.Awake();
        _selfReleaser = GetComponent<BotReleaser>();
    }

    private void OnDisable()
    {
        Jump.ResetJump();
    }

    public void Activate()
    {
        _selfReleaser.Activate();
    }

    protected override void OutOfBounds()
    {
        if (Dead) return;

        Dead = true;
        _selfReleaser.Dead = true;

        _selfReleaser.ReleaseControls(false);
    }
}
