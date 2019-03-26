using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGrill : MonoBehaviour, IButtonInteraction
{
    [SerializeField, Tooltip("The time between fires")]
    private float _fireInterval = 2;
    [SerializeField, Tooltip("The length of time that the fire burns for")]
    private float _fireDuration = 1;
    [SerializeField, Tooltip("The delay after activated until the fire cycle starts")]
    private float _startDelay = 0;
    [SerializeField, Tooltip("The gameobject under which are all the flames")]
    private GameObject _flames = null;
    [SerializeField]
    private bool _activated = true;
    private PhysicsRepeatingTimer _timer;
    private Collider _trigger;
    private bool _cycleStarted = false;

    private void Awake()
    {
        _timer = gameObject.AddComponent<PhysicsRepeatingTimer>();
        _trigger = GetComponent<Collider>();
    }

    private void Start()
    {
        if (_startDelay > 0)
        {
            _timer.OnTimerCompleted += DelayedStartCycle;
            _timer.StartTimer(_startDelay);
        }
        else
        {
            StartCycle();
        }

        if (_activated)
        {
            FlamesOn();
        }
        else
        {
            FlamesOff();
            _timer.StopTimer();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_cycleStarted && _trigger.enabled && _fireInterval > 0.0f && _timer.TimeElapsed > _fireDuration)
        {
            FlamesOff();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<IDamageReceiver>().Die();
    }

    private void FlamesOn()
    {
        _trigger.enabled = true;
        _flames.SetActive(true);
    }

    private void FlamesOff()
    {
        _trigger.enabled = false;
        _flames.SetActive(false);
    }

    private void StartCycle()
    {
        _timer.OnTimerCompleted += FlamesOn;
        _timer.StartTimer(_fireDuration + _fireInterval);
        FlamesOn();
        _cycleStarted = true;
    }

    private void DelayedStartCycle()
    {
        _timer.StopTimer();
        _timer.OnTimerCompleted -= DelayedStartCycle;
        StartCycle();
    }

    public void ButtonDown()
    {
        _activated = false;
        FlamesOff();
        _timer.StopTimer();
    }

    public void ButtonUp()
    {
        _activated = true;
        if (!_cycleStarted || _timer.TimeElapsed <= _fireDuration)
        {
            FlamesOn();
        }
        _timer.ResumeTimer();
    }
}
