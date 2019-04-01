using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineAction : BotActionBase
{
    [SerializeField]
    private string _sTrampolineButton = "Stay Action";
    private GameObject _goTrampoline;

    protected override void Awake()
    {
        base.Awake();
        _goTrampoline = GetComponentInChildren<TopOfThetramp>(true).gameObject;
    }

    void Update()
    {
        if (Input.GetButtonDown(_sTrampolineButton))
        {
            _goTrampoline.SetActive(true);
            // _ostRelease.StopTimer();
            // _ostLife.StartTimer(_fLifeTime);
            _selfMover._animator.SetTrigger("Trampoline");
            _pbi.ReleaseControls(true);
        }
    }

    public override void DisableAction()
    {
        _goTrampoline.SetActive(false);
    }
}
