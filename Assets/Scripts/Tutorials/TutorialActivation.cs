using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialActivation : MonoBehaviour
{
    [SerializeField, Tooltip("0 means no timer")]
    private float _hideTime = 0.0f;
    [SerializeField, Tooltip("Put all button names that can hide prompt here")]
    private string[] _hideButtons = new string[1];
    [SerializeField, Tooltip("Put all axis names that can hide prompt here")]
    private string[] _hideAxes = new string[1];
    [SerializeField, Range(0.0f, 1.0f)]
    private float _axisThreshold = 0.5f;
    /* [SerializeField]
    private bool _hideByMovement = false;
    [SerializeField]
    private bool _hideByCamera = false;
    [SerializeField]
    private bool _hideByJump = false;
    [SerializeField]
    private bool _hideByDeploy = false;
    [SerializeField]
    private bool _hideByHack = false;
    [SerializeField]
    private bool _hideByBomb = false;
    [SerializeField]
    private bool _hideByTramp = false; */
    [SerializeField]
    private bool _followPlayer = true;
    [SerializeField]
    private string _attachPointName = "Tutorial Attach Point";

    private Collider _collider = null;
    private ScaledOneShotTimer _timer = null;
    private CanvasLookAtCamera _prompt = null;
    private Transform _attachPoint = null;
    private bool _shown = false;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _prompt = GetComponentInChildren<CanvasLookAtCamera>();

        if (_hideTime > 0.0f)
        {
            _timer = gameObject.AddComponent<ScaledOneShotTimer>();
            _timer.OnTimerCompleted += MyWorkHereIsDone;
        }
    }

    private void Start()
    {
        _prompt.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= MyWorkHereIsDone;
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        if (!_shown) return;

        if (_hideButtons != null && _hideButtons.Length > 0)
        {
            foreach (string button in _hideButtons)
            {
                if (!button.Equals("") && Input.GetButtonDown(button))
                {
                    if (_timer != null) _timer.StopTimer();
                    MyWorkHereIsDone();
                    return;
                }
            }
        }
        else if (_hideAxes != null && _hideAxes.Length > 0)
        {
            foreach (string axis in _hideAxes)
            {
                if (!axis.Equals("") && Mathf.Abs(Input.GetAxis(axis)) >= _axisThreshold)
                {
                    if (_timer != null) _timer.StopTimer();
                    MyWorkHereIsDone();
                    return;
                }
            }
        }

        if (_followPlayer)
        {
            _prompt.transform.position = _attachPoint.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerMovement mover = other.GetComponent<PlayerMovement>();
        if (mover == null || mover.ControlsDisabled) return;

        _collider.enabled = false;
        if (_followPlayer)
        {
            _attachPoint = other.transform.Find(_attachPointName);
        }
        _prompt.gameObject.SetActive(true);
        _shown = true;

        if (_hideTime > 0.0f) _timer.StartTimer(_hideTime);
    }

    private void MyWorkHereIsDone()
    {
        gameObject.SetActive(false);
    }
}