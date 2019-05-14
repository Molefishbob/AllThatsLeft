using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : PlayerMovement
{
    public bool Dead { get { return _selfReleaser.Dead; } }
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
        _selfReleaser.DeadButNotDead();
        _selfReleaser.ReleaseControls(false);
    }
}
