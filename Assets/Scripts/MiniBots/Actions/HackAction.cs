using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackAction : BotActionBase
{
    [SerializeField]
    private string _sHackButton = "Hack Action";
    public LayerMask HackLayer { get { return _lHackLayer; } }
    [SerializeField]
    private LayerMask _lHackLayer = 1 << 18;
    private List<GenericHackable> _hackTargets = null;
    public bool Hacking { get { return _bHacking; } }
    private bool _bHacking = false;
    private BotReleaser _releaser = null;

    protected override void Awake()
    {
        base.Awake();
        _releaser = GetComponent<BotReleaser>();
        _hackTargets = new List<GenericHackable>(4);
    }

    private void Update()
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

        if (_bHacking)
        {
            if (_hackTargets[0].CurrentStatus == GenericHackable.Status.Hacked)
            {
                _selfMover._animator.SetBool("Hack", false);
                _hackTargets.Clear();
                _bHacking = false;
            }
            return;
        }

        if (!_selfMover.IsGrounded || !_bCanAct) return;

        if (Input.GetButtonDown(_sHackButton))
        {
            if (_hackTargets != null && _hackTargets.Count > 0)
            {
                if (_hackTargets[0].CurrentStatus == GenericHackable.Status.NotHacked)
                {
                    _selfMover._animator.SetBool("Hack", true);
                    _bHacking = true;
                    _hackTargets[0].TimeToStart();
                    _releaser.DisableActing();
                    _releaser.ReleaseControls(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GenericHackable ghOther = other.GetComponent<GenericHackable>();
        if (ghOther == null) return;
        _hackTargets.Add(ghOther);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_bHacking) return;
        GenericHackable ghOther = other.GetComponent<GenericHackable>();
        if (ghOther == null) return;
        _hackTargets.Remove(ghOther);
    }

    public override void DisableAction()
    {
        _bHacking = false;
    }
}
