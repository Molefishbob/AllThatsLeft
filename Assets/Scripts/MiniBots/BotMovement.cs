using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : PlayerMovement
{
    public bool Dead { get { return _selfReleaser.Dead; } }
    private BotReleaser _selfReleaser;

    public override bool ControlsDisabled
    {
        get
        {
            return _controlsDisabled || Dead;
        }
        set
        {
            if (!Dead)
            {
                _controlsDisabled = value;
                if (Jump != null)
                {
                    Jump.ControlsDisabled = value;
                }
                if (_controlsDisabled)
                {
                    ResetInternalMove();
                }
            }
        }
    }

    public float TurningSpeed { get { return _turningSpeed; } }

    protected override void Awake()
    {
        base.Awake();
        _selfReleaser = GetComponent<BotReleaser>();
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
