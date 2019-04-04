using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineAction : BotActionBase
{
    [SerializeField]
    private string _sTrampolineButton = "Stay Action";
    private GameObject _goTrampoline;
    private BotReleaser _releaser;
    public bool _bActing = false;

    protected override void Awake()
    {
        base.Awake();
        _goTrampoline = GetComponentInChildren<TopOfThetramp>(true).gameObject;
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

        if (Input.GetButtonDown(_sTrampolineButton))
        {
            _goTrampoline.SetActive(true);
            _bActing = true;
            _selfMover._animator.SetTrigger("Trampoline");
            _releaser.ReleaseControls(true);
        }
    }

    public override void DisableAction()
    {
        _goTrampoline.SetActive(false);
        _bActing = false;
    }
}
