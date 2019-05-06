using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour, IButtonInteraction
{
    private const int PlayerLayer = 10;
    [SerializeField, Tooltip("Add the object that is affected by the button being pressed.\nMust Implement IButtonIntercation -Interface!")]
    private MonoBehaviour _target = null;
    [SerializeField]
    private float _cooldownDuration = 1;
    private IButtonInteraction _tInt;
    private BoxCollider _coll;
    private bool _isButtonPressed;
    private PhysicsOneShotTimer _timer;
    private bool _cooldownDone = true;
    private bool _buttonOnHold;

    /// <summary>
    /// Awake is called before the first frame update
    /// </summary>
    void Awake()
    {
        _timer = GetComponent<PhysicsOneShotTimer>();
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

    private void Start()
    {
        _timer.OnTimerCompleted += CompleteCooldown;
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= CompleteCooldown;
        }
    }

    /// <summary>
    /// What happens when the button is pressed down
    /// </summary>
    public bool ButtonDown()
    {
        /// TODO: ADD ANIMATION
        /// TODO: ADD SOUNDS HERE
        _tInt.ButtonDown();
        _timer.StartTimer(_cooldownDuration);
        _cooldownDone = false;
        return true;
    }

    /// <summary>
    /// What happens when the button is no longer pressed down
    /// </summary>
    public bool ButtonUp()
    {
        /// TODO: ADD ANIMATION
        /// TODO: ADD SOUNDS HERE
        _tInt.ButtonUp();
        _timer.StartTimer(_cooldownDuration);
        _cooldownDone = false;
        return true;
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

    private void CompleteCooldown()
    {
        _cooldownDone = true;
    }
}
