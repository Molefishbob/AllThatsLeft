using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GassableUnit : MonoBehaviour
{
    [SerializeField]
    private RandomSFXSound _coughSound = null;
    [SerializeField]
    private float _blinkInterval = 0.25f;
    private Color _gasTint;
    private Color[][] _originalColors;
    private bool _tintingBack = false;
    private float _deathTintStrength;
    private PhysicsOneShotTimer _killTimer;
    private ScaledRepeatingTimer _blinkTimer;
    private IDamageReceiver _dmg;
    private SkinnedMeshRenderer[] _renderers;

    private void Awake()
    {
        _killTimer = gameObject.AddComponent<PhysicsOneShotTimer>();
        _blinkTimer = gameObject.AddComponent<ScaledRepeatingTimer>();
        _dmg = GetComponent<IDamageReceiver>();
        _renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        _originalColors = new Color[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = new Color[_renderers[i].materials.Length];
            for (int j = 0; j < _renderers[i].materials.Length; j++)
            {
                _originalColors[i][j] = _renderers[i].materials[j].color;
            }
        }
    }

    private void Start()
    {
        _killTimer.OnTimerCompleted += Death;
        _blinkTimer.OnTimerCompleted += SwitchDirection;
    }

    private void OnDisable()
    {
        ExitGas();
    }

    private void OnDestroy()
    {
        if (_killTimer != null && _dmg != null)
        {
            _killTimer.OnTimerCompleted -= Death;
        }
        if (_blinkTimer != null) _blinkTimer.OnTimerCompleted -= SwitchDirection;
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_blinkTimer.IsRunning)
        {
            if (!_coughSound.IsPlaying) _coughSound.PlaySound();
            if (_tintingBack)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    for (int j = 0; j < _renderers[i].materials.Length; j++)
                    {
                        _renderers[i].materials[j].color = Color.Lerp(_gasTint, _originalColors[i][j], _blinkTimer.NormalizedTimeElapsed);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    for (int j = 0; j < _renderers[i].materials.Length; j++)
                    {
                        _renderers[i].materials[j].color = Color.Lerp(_originalColors[i][j], _gasTint, _blinkTimer.NormalizedTimeElapsed);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Start timer when entering gas.
    /// </summary>
    /// <param name="oofTime">Time until oof</param>
    public void EnterGas(float oofTime, Color tint, float deathTintStrength)
    {
        _gasTint = tint;
        _deathTintStrength = deathTintStrength;
        _tintingBack = false;
        _killTimer.StartTimer(oofTime);
        _blinkTimer.StartTimer(_blinkInterval);
        EnterGasExtras();
    }

    protected virtual void EnterGasExtras() { }

    /// <summary>
    /// Stop timer when leaving gas.
    /// </summary>
    public void ExitGas()
    {
        _killTimer.StopTimer();
        _blinkTimer.StopTimer();
        for (int i = 0; i < _renderers.Length; i++)
        {
            for (int j = 0; j < _renderers[i].materials.Length; j++)
            {
                _renderers[i].materials[j].color = _originalColors[i][j];
            }
        }
        ExitGasExtras();
    }

    protected virtual void ExitGasExtras() { }

    private void Death()
    {
        _killTimer.StopTimer();
        _blinkTimer.StopTimer();
        for (int i = 0; i < _renderers.Length; i++)
        {
            for (int j = 0; j < _renderers[i].materials.Length; j++)
            {
                _renderers[i].materials[j].color = Color.Lerp(_originalColors[i][j], _gasTint, _deathTintStrength);
            }
        }
        ExitGasExtras();
        _dmg.Die();
    }

    private void SwitchDirection()
    {
        _tintingBack = !_tintingBack;
    }
}
