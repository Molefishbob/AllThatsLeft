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
    [SerializeField, Tooltip("How much time is left when sparks are activated")]
    private float _sparkActivationtime = 0.2f;
    [SerializeField, Tooltip("The gameobject under which are all the flames")]
    private GameObject _flames = null;
    [SerializeField, Tooltip("Warning of incoming fire")]
    private GameObject _sparks = null;
    [SerializeField]
    private bool _activated = true;
    [SerializeField, Tooltip("Time it waits before deactivating when hacked")]
    private float _deactivationTime = 0.5f;
    private PhysicsRepeatingTimer _timer;
    private Collider _trigger;
    private bool _cycleStarted = false;
    private ScaledOneShotTimer _deactivationTimer;
    [Tooltip("The duration after which the symbol goes off")]
    public float _delayDuration = 0.4f;
    [SerializeField]
    protected GameObject _symbol = null;
    [SerializeField]
    protected SingleSFXSound _sparkSound = null,
                             _flameBurstSound = null,
                             _FlameLoopSound = null;
    protected ScaledOneShotTimer _delayTimer;

    private void Awake()
    {
        _timer = gameObject.AddComponent<PhysicsRepeatingTimer>();
        _trigger = GetComponent<Collider>();
        _deactivationTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _delayTimer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Start()
    {
        _deactivationTimer.OnTimerCompleted += FlamesOff;
        _delayTimer.OnTimerCompleted += SymbolDown;

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

        if (_cycleStarted && _trigger.enabled && _fireInterval > 0.0f && _timer.TimeElapsed > _fireDuration + _sparkActivationtime)
        {
            FlamesOff();
        }
        if (_cycleStarted && !_trigger.enabled && _fireInterval > 0.0f && _timer.TimeElapsed < _fireDuration + _sparkActivationtime && _timer.TimeElapsed > _sparkActivationtime)
        {
            FlamesOn();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<IDamageReceiver>().Die();
    }

    private void FlamesOn()
    {
        _sparkSound.StopSound();
        _flameBurstSound.PlaySound();
        _FlameLoopSound.PlaySound();
        _sparks.SetActive(false);
        _trigger.enabled = true;
        _flames.SetActive(true);
    }

    private void FlamesOff()
    {
        _trigger.enabled = false;
        _FlameLoopSound.StopSound();
        _flames.SetActive(false);
    }

    private void SparksOn()
    {
        if (!_sparks.activeSelf && !_flames.activeSelf)
        {
            _sparks.SetActive(true);
            _sparkSound.PlaySound();
        }
    }

    private void StartCycle()
    {
        _timer.OnTimerCompleted += SparksOn;
        _timer.StartTimer(_fireDuration + _fireInterval);

        if (_fireInterval > 0.0f)
        {
            SparksOn();
        }
        else
        {
            FlamesOn();
        }

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
        _delayTimer.StartTimer(_delayDuration);
        _timer.StopTimer();
        _deactivationTimer.StartTimer(_deactivationTime);
    }

    public void ButtonUp()
    {
        _activated = true;
        if (!_cycleStarted || _timer.TimeElapsed <= _fireDuration)
        {
            FlamesOn();
        }
        _delayTimer.StartTimer(_delayDuration);
        _timer.ResumeTimer();
    }
    protected virtual void SymbolDown()
    {
        try
        {
            _symbol.SetActive(false);
        }
        catch
        {
            Debug.LogError(gameObject.name + " has to have a symbol!");
        }
    }

    private void OnDestroy()
    {
        if (_deactivationTimer != null)
            _deactivationTimer.OnTimerCompleted -= FlamesOff;
    }
}
