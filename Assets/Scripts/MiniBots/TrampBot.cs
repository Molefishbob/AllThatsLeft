using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampBot : GenericBot
{
    public float _fMaxJumpHeight = 10;
    const int _iPlayerLayer = 10;
    private PlayerJump _PlayerMover;
    private PlayerMovement _Player;
    //private bool _bCanBounce = false;
    private TopOfThetramp tott;

    protected override void Awake()
    {
        base.Awake();
        tott = GetComponentInChildren<TopOfThetramp>();
        tott.MaxJumpHeight = _fMaxJumpHeight;
    }
    public override void StartMovement()
    {
        base.StartMovement();
        tott.CanBounce = true;
    }

    public override void ResetBot()
    {
        base.ResetBot();
        tott.CanBounce = false;
    }
}
