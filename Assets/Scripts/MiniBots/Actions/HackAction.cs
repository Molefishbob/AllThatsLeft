using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackAction : BotActionBase
{
    public event GenericEvent OnHackUsed;

    [SerializeField]
    private string _sHackButton = "Hack Action";
    public LayerMask HackLayer { get { return _lHackLayer; } }
    [SerializeField]
    private LayerMask _lHackLayer = 1 << 18;
    private GameObject[] _goHackTarget = null;
    public bool Hacking { get { return _bHacking; } }
    private bool _bHacking = false;
    private BotReleaser _releaser = null;

    protected override void Awake()
    {
        base.Awake();
        _releaser = GetComponent<BotReleaser>();
    }

    void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            _bPaused = true;
            return;
        }
        if (_bPaused)
        {
            _bPaused = false;
            return;
        }

        if (!_selfMover.IsGrounded || !_bCanAct) return;

        if (Input.GetButtonDown(_sHackButton))
        {
            _goHackTarget = CheckSurroundings(_lHackLayer, _selfMover._controller.radius + _selfMover._controller.skinWidth);
            if (_goHackTarget != null)
            {
                GenericHackable ghOther = _goHackTarget[0].GetComponent<GenericHackable>();
                if (ghOther._currentStatus == GenericHackable.Status.NotHacked)
                {
                    _bHacking = true;
                    ghOther.TimeToStart();
                    _releaser.ReleaseControls(true);
                    if (OnHackUsed != null) OnHackUsed();
                }
                _selfMover._animator.SetBool("Hack", true);
            }
        }
    }

    public override void DisableAction()
    {
        _bHacking = false;
    }
}
