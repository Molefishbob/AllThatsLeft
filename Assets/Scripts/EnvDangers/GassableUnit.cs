using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GassableUnit : MonoBehaviour
{
    [SerializeField]
    private Canvas _poisonScreen = null;
    [SerializeField]
    private float _blinkInterval = 0.25f;
    [SerializeField]
    private Color _gasTint = Color.magenta;
    private Color[] _originalColors;
    private bool _tintingBack = false;
    private PhysicsOneShotTimer _killTimer;
    private ScaledRepeatingTimer _blinkTimer;
    private IDamageReceiver _dmg;
    private Renderer _renderer;

    private void Awake()
    {
        _killTimer = gameObject.AddComponent<PhysicsOneShotTimer>();
        _blinkTimer = gameObject.AddComponent<ScaledRepeatingTimer>();
        _dmg = GetComponent<IDamageReceiver>();
        _renderer = GetComponentInChildren<Renderer>();
        _originalColors = new Color[_renderer.materials.Length];
        for (int i = 0; i < _originalColors.Length; i++)
        {
            _originalColors[i] = _renderer.materials[i].color;
        }
    }

    private void OnEnable()
    {
        ExitGas();
    }

    private void Start()
    {
        _killTimer.OnTimerCompleted += Death;
        _blinkTimer.OnTimerCompleted += SwapColors;
    }

    private void OnDestroy()
    {
        if (_killTimer != null && _dmg != null)
        {
            _killTimer.OnTimerCompleted -= Death;
        }
        if (_blinkTimer != null) _blinkTimer.OnTimerCompleted -= SwapColors;
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_blinkTimer.IsRunning)
        {
            if (_tintingBack)
            {
                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].color = Color.Lerp(_gasTint, _originalColors[i], _blinkTimer.NormalizedTimeElapsed);
                }
            }
            else
            {
                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].color = Color.Lerp(_originalColors[i], _gasTint, _blinkTimer.NormalizedTimeElapsed);
                }
            }
        }
    }

    /// <summary>
    /// Start timer when entering gas.
    /// </summary>
    /// <param name="oofTime">Time until oof</param>
    public void EnterGas(float oofTime)
    {
        _tintingBack = false;
        if (_poisonScreen != null) _poisonScreen.gameObject.SetActive(true);
        _killTimer.StartTimer(oofTime);
        _blinkTimer.StartTimer(_blinkInterval);
    }

    /// <summary>
    /// Stop timer when leaving gas.
    /// </summary>
    public void ExitGas()
    {
        _killTimer.StopTimer();
        _blinkTimer.StopTimer();
        for (int i = 0; i < _renderer.materials.Length; i++)
        {
            _renderer.materials[i].color = _originalColors[i];
        }
        if (_poisonScreen != null) _poisonScreen.gameObject.SetActive(false);
    }

    private void Death()
    {
        _killTimer.StopTimer();
        _blinkTimer.StopTimer();
        for (int i = 0; i < _renderer.materials.Length; i++)
        {
            _renderer.materials[i].color = Color.Lerp(_gasTint, _originalColors[i], 0.5f);
        }
        if (_poisonScreen != null) _poisonScreen.gameObject.SetActive(false);
        _dmg.Die();
    }

    private void SwapColors()
    {
        _tintingBack = !_tintingBack;
    }
}
