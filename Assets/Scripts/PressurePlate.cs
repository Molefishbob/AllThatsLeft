﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour, IButtonInteraction, ITimedAction
{
    private const int PlayerLayer = 10;
    [SerializeField, Tooltip("Add the object that is affected by the button being pressed.\nMust Implement IButtonIntercation -Interface!")]
    private MonoBehaviour _target = null;
    [SerializeField]
    private float _cooldownDuration = 1;
    private IButtonInteraction _tInt;
    private BoxCollider _coll;
    private bool _isButtonPressed;
    private OneShotTimer _timer;
    private bool _cooldownDone = true;
    private bool _buttonOnHold;

    /// <summary>
    /// Awake is called before the first frame update
    /// </summary>
    void Awake()
    {
        _timer = GetComponent<OneShotTimer>();
        _timer.SetTimerTarget(this);
        _coll = GetComponent<BoxCollider>();
        try
        {
            _tInt = (IButtonInteraction) _target;
        }
        catch
        {
            Debug.LogError("The Target of " + gameObject.name + " " + transform.position + " HAS to implement IButtonInteraction");
        }
    }

    /// <summary>
    /// What happens when the button is pressed down
    /// </summary>
    public void ButtonDown()
    {
        /// TODO: ADD ANIMATION
        /// TODO: ADD SOUNDS HERE
        _tInt.ButtonDown();
        _timer.StartTimer(_cooldownDuration);
        _cooldownDone = false;
    }

    /// <summary>
    /// What happens when the button is no longer pressed down
    /// </summary>
    public void ButtonUp()
    {
        /// TODO: ADD ANIMATION
        /// TODO: ADD SOUNDS HERE
        _tInt.ButtonUp();
        _timer.StartTimer(_cooldownDuration);
        _cooldownDone = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isButtonPressed && _cooldownDone)
        {
            ButtonDown();
            _isButtonPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isButtonPressed )
        {
            ButtonUp();
            _isButtonPressed = false;
        }
    }

    public void TimedAction()
    {
        _cooldownDone = true;
    }
}